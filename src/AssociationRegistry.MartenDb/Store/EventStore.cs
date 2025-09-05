namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.EventStore.ConflictResolution;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using DecentraalBeheer.Vereniging;
using JasperFx.Events;
using Marten;
using Microsoft.Extensions.Logging;
using NodaTime.Text;
using IEvent = Events.IEvent;
using IEventStore = Store.IEventStore;

public class EventStore : Store.IEventStore
{
    public class ExpectedVersion
    {
        public const long NewStream = 0;
    }

    private readonly IDocumentStore _documentStore;
    private readonly EventConflictResolver _conflictResolver;
    private readonly ILogger<EventStore> _logger;

    public EventStore(IDocumentStore documentStore, EventConflictResolver conflictResolver, ILogger<EventStore> logger)
    {
        _documentStore = documentStore;
        _conflictResolver = conflictResolver;
        _logger = logger;
    }

    public async Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default,
        params IEvent[] events)
    {
        await using var session = _documentStore.LightweightSession();

        return await Save(aggregateId, aggregateVersion, session, metadata, cancellationToken, events);
    }

    public async Task<StreamActionResult> SaveNew(
        string aggregateId,
        long aggregateVersion,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events)
    {
        SetHeaders(metadata, session);

        TryLockForKboNumber(aggregateId, session, events.FirstOrDefault());

        var streamAction = session.Events.StartStream(aggregateId, events);

        await session.SaveChangesAsync(cancellationToken);

        var maxSequence = streamAction.Events.Max(@event => @event.Sequence);

        if(maxSequence == 0)
            maxSequence = (await session.Events.FetchStreamAsync(aggregateId, token: cancellationToken)).Max(x => x.Sequence);

        return new StreamActionResult(maxSequence, events.Length);
    }

    public async Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events)
    {
        try
        {
            SetHeaders(metadata, session);

            TryLockForKboNumber(aggregateId, session, events.FirstOrDefault());

            var streamAction = AppendEvents(session, aggregateId, events, aggregateVersion);

            await session.SaveChangesAsync(cancellationToken);

            var eventsAgain = await session.Events.FetchStreamAsync(aggregateId, token: cancellationToken);
            var maxSequence = eventsAgain.Max(@event => @event.Sequence);

            _logger.LogInformation("SAVED EVENTS {@EventNames} with max sequence: {MaxSeq}", string.Join(", ", events.Select(x => x.GetType().Name)), maxSequence);

            return new StreamActionResult(eventsAgain.Max(@event => @event.Sequence), eventsAgain.Max(x => x.Version));
        }
        catch (EventStreamUnexpectedMaxEventIdException)
        {
            var eventsDiff =
                await session.Events.FetchStreamAsync(aggregateId, fromVersion: aggregateVersion + 1,
                                                      token: cancellationToken);

            if (_conflictResolver.IsAllowedPostConflict(events, eventsDiff))
            {
                session.EjectAllPendingChanges();

                var streamAction = session.Events.Append(aggregateId, aggregateVersion + events.Length + eventsDiff.Count,
                                                         events);

                await session.SaveChangesAsync(cancellationToken);

                var eventsAgain = await session.Events.FetchStreamAsync(aggregateId, token: cancellationToken);
                var maxSequence = eventsAgain.Max(@event => @event.Sequence);

                _logger.LogInformation("SAVED EVENTS {@EventNames} with max sequence: {MaxSeq}", string.Join(", ", events.Select(x => x.GetType().Name)), maxSequence);
                return new StreamActionResult(eventsAgain.Max(@event => @event.Sequence), eventsAgain.Max(x => x.Version));
                //return new StreamActionResult(maxSequence, streamAction.Version);
            }

            throw new UnexpectedAggregateVersionException();
        }
    }

    private static void TryLockForKboNumber(string vCode, IDocumentSession session, IEvent? registreerEvent)
    {
        if (registreerEvent is VerenigingMetRechtspersoonlijkheidWerdGeregistreerd evnt)
            session.Events.StartStream<KboNummer>(evnt.KboNummer, new KboNummerWerdGereserveerd(vCode));
    }

    private static StreamAction AppendEvents(
        IDocumentSession session,
        string aggregateId,
        IReadOnlyCollection<IEvent> events,
        long? expectedVersion)
    {
        if (expectedVersion is not null)
            return session.Events.Append(aggregateId, expectedVersion.Value + events.Count, events);

        return session.Events.Append(aggregateId, events);
    }

    private static void SetHeaders(CommandMetadata metadata, IDocumentSession session)
    {
        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        session.CorrelationId = metadata.CorrelationId.ToString();
    }

    public async Task<T> Load<T>(string id, long? expectedVersion) where T : class, IHasVersion, new()
    {
        await using var session = _documentStore.LightweightSession();

        var aggregate = await session.Events.AggregateStreamAsync<T>(id) ??
                        throw new AggregateNotFoundException(id, typeof(T));

        if (expectedVersion is not null && aggregate.Version != expectedVersion)
        {
            Throw<UnexpectedAggregateVersionException>.If(expectedVersion > aggregate.Version);

            var eventsInNewVersion = (await session.Events.FetchStreamAsync(id, aggregate.Version))
               .Where(x => x.Version > expectedVersion);

            if (!_conflictResolver.IsAllowedPreConflict(eventsInNewVersion))
                throw new UnexpectedAggregateVersionException();
        }

        return aggregate;
    }

    public async Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion) where T : class, IHasVersion, new()
    {
        var vCode = await GetVCodeForKbo(kboNummer);

        if (vCode is null)
            throw new AggregateNotFoundException(kboNummer, typeof(T));

        return await Load<T>(vCode!, expectedVersion);
    }

    public async Task<bool> Exists(VCode vCode)
    {
        await using var session = _documentStore.LightweightSession();
        var streamState = await session.Events.FetchStreamStateAsync(vCode);

        return streamState != null;
    }

    public async Task<bool> Exists(KboNummer kboNummer)
    {
        await using var session = _documentStore.LightweightSession();
        var streamState = await session.Events.FetchStreamStateAsync(kboNummer);

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            await session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                         .Where(x => x.KboNummer == kboNummer.Value)
                         .SingleOrDefaultAsync();

        return streamState != null ||
               verenigingMetRechtspersoonlijkheidWerdGeregistreerd != null;
    }

    public async Task<VCode?> GetVCodeForKbo(string kboNummer)
    {
        await using var session = _documentStore.LightweightSession();

        var id = (await session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                               .Where(geregistreerd => geregistreerd.KboNummer == kboNummer)
                               .SingleOrDefaultAsync())?.VCode;

        if (string.IsNullOrEmpty(id))
            return null;

        return VCode.Hydrate(id);
    }

}
