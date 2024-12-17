namespace AssociationRegistry.EventStore;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Events;
using Framework;
using Marten;
using Marten.Events;
using Marten.Exceptions;
using Microsoft.Extensions.Logging;
using NodaTime.Text;
using Vereniging;
using IEvent = Framework.IEvent;

public class EventStore : IEventStore
{
    public const string DigitaalVlaanderenOvoNumber = "OVO002949";
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
        CommandMetadata metadata,
        CancellationToken cancellationToken = default,
        params IEvent[] events)
    {
        await using var session = _documentStore.LightweightSession();

        return await Save(aggregateId, session, metadata, cancellationToken, events);
    }

    public async Task<StreamActionResult> Save(
        string aggregateId,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events)
    {
        try
        {
            SetHeaders(metadata, session);

            TryLockForKboNumber(aggregateId, session, events.FirstOrDefault());

            var streamAction = AppendEvents(session, aggregateId, events, metadata.ExpectedVersion);

            await session.SaveChangesAsync(cancellationToken);

            var maxSequence = streamAction.Events.Max(@event => @event.Sequence);

            if(maxSequence < 1)
                _logger.LogWarning("Sequence is less than expected: {Sequence}", maxSequence);

            var eventsAgain = session.Events.FetchStream(aggregateId);
            return new StreamActionResult(eventsAgain.Max(@event => @event.Sequence), eventsAgain.Max(x => x.Version));
            //return new StreamActionResult(maxSequence, streamAction.Version);
        }
        catch (EventStreamUnexpectedMaxEventIdException)
        {
            var eventsDiff =
                await session.Events.FetchStreamAsync(aggregateId, fromVersion: metadata.ExpectedVersion.Value + 1,
                                                      token: cancellationToken);

            if (_conflictResolver.IsAllowedPostConflict(events, eventsDiff))
            {
                session.EjectAllPendingChanges();

                var streamAction = session.Events.Append(aggregateId, metadata.ExpectedVersion.Value + events.Length + eventsDiff.Count,
                                                         events);

                await session.SaveChangesAsync(cancellationToken);

                var maxSequence = streamAction.Events.Max(@event => @event.Sequence);

                if(maxSequence < 1)
                    _logger.LogWarning("Sequence is less than expected: {Sequence}", maxSequence);

                var eventsAgain = session.Events.FetchStream(aggregateId);
                return new StreamActionResult(eventsAgain.Max(@event => @event.Sequence), eventsAgain.Max(x => x.Version));
                //return new StreamActionResult(maxSequence, streamAction.Version);
            }

            throw new UnexpectedAggregateVersionException();
        }
    }

    private static void TryLockForKboNumber(string vCode, IDocumentSession session, IEvent? registreerEvent)
    {
        if (registreerEvent is VerenigingMetRechtspersoonlijkheidWerdGeregistreerd evnt)
            session.Events.StartStream<KboNummer>(evnt.KboNummer, new { VCode = vCode });
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
            var eventsInNewVersion = (await session.Events.FetchStreamAsync(id, aggregate.Version))
               .Where(x => x.Version > expectedVersion);

            if (!_conflictResolver.IsAllowedPreConflict(eventsInNewVersion))
                throw new UnexpectedAggregateVersionException();
        }

        return aggregate;
    }

    public async Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion) where T : class, IHasVersion, new()
    {
        await using var session = _documentStore.LightweightSession();

        var id = (await session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                               .Where(geregistreerd => geregistreerd.KboNummer == kboNummer)
                               .SingleOrDefaultAsync())?.VCode;

        if (string.IsNullOrEmpty(id))
            throw new AggregateNotFoundException(kboNummer, typeof(T));

        return await Load<T>(id, expectedVersion);
    }

    public async Task<bool> Exists(VCode vCode)
    {
        await using var session = _documentStore.LightweightSession();
        var streamState = await session.Events.FetchStreamStateAsync(vCode);

        return streamState != null;
    }
}
