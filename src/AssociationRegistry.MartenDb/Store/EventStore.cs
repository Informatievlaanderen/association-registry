namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using AssociationRegistry.EventStore.ConflictResolution;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using DecentraalBeheer.Vereniging;
using Events;
using Framework;
using JasperFx.Events;
using Marten;
using Microsoft.Extensions.Logging;
using NodaTime.Text;
using Persoonsgegevens;
using IEvent = Events.IEvent;

public class EventStore : IEventStore
{
    public class ExpectedVersion
    {
        public const long NewStream = 0;
    }

    private readonly IDocumentSession _session;
    private readonly EventConflictResolver _conflictResolver;
    private readonly IVertegenwoordigerPersoonsgegevensRepository _vertegenwoordigerPersoonsgegevensRepository;
    private readonly ILogger<EventStore> _logger;

    public EventStore(
        IDocumentSession session,
        EventConflictResolver conflictResolver,
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository,
        ILogger<EventStore> logger)
    {
        _session = session;
        _conflictResolver = conflictResolver;
        _vertegenwoordigerPersoonsgegevensRepository = vertegenwoordigerPersoonsgegevensRepository;
        _logger = logger;
    }

    public async Task<StreamActionResult> SaveNew(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events)
    {
        await SplitPersoonsgegevensFromEvents(aggregateId, events);

        SetHeaders(metadata, _session);

        TryLockForKboNumber(aggregateId, _session, events.FirstOrDefault());

        var streamAction = _session.Events.StartStream(aggregateId, events);

        await _session.SaveChangesAsync(cancellationToken);

        var maxSequence = streamAction.Events.Max(@event => @event.Sequence);

        if (maxSequence == 0)
            maxSequence = (await _session.Events.FetchStreamAsync(aggregateId, token: cancellationToken)).Max(x => x.Sequence);

        return new StreamActionResult(maxSequence, events.Length);
    }

    public async Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events)
    {
        try
        {
            await SplitPersoonsgegevensFromEvents(aggregateId, events);

            SetHeaders(metadata, _session);

            TryLockForKboNumber(aggregateId, _session, events.FirstOrDefault());

            var streamAction = AppendEvents(_session, aggregateId, events, aggregateVersion);

            await _session.SaveChangesAsync(cancellationToken);

            var eventsAgain = await _session.Events.FetchStreamAsync(aggregateId, token: cancellationToken);
            var maxSequence = eventsAgain.Max(@event => @event.Sequence);

            _logger.LogInformation("SAVED EVENTS {@EventNames} with max sequence: {MaxSeq}",
                                   string.Join(", ", events.Select(x => x.GetType().Name)), maxSequence);

            return new StreamActionResult(eventsAgain.Max(@event => @event.Sequence), eventsAgain.Max(x => x.Version));
        }
        catch (EventStreamUnexpectedMaxEventIdException)
        {
            var eventsDiff =
                await _session.Events.FetchStreamAsync(aggregateId, fromVersion: aggregateVersion + 1,
                                                       token: cancellationToken);

            if (_conflictResolver.IsAllowedPostConflict(events, eventsDiff))
            {
                _session.EjectAllPendingChanges();

                var streamAction = _session.Events.Append(aggregateId, aggregateVersion + events.Length + eventsDiff.Count,
                                                          events);

                await _session.SaveChangesAsync(cancellationToken);

                var eventsAgain = await _session.Events.FetchStreamAsync(aggregateId, token: cancellationToken);
                var maxSequence = eventsAgain.Max(@event => @event.Sequence);

                _logger.LogInformation("SAVED EVENTS {@EventNames} with max sequence: {MaxSeq}",
                                       string.Join(", ", events.Select(x => x.GetType().Name)), maxSequence);

                return new StreamActionResult(eventsAgain.Max(@event => @event.Sequence), eventsAgain.Max(x => x.Version));
                //return new StreamActionResult(maxSequence, streamAction.Version);
            }

            throw new UnexpectedAggregateVersionException();
        }
    }

    private async Task SplitPersoonsgegevensFromEvents(string aggregateId, IEvent[] events)
    {
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i] is VertegenwoordigerWerdToegevoegd oldEvent)
            {
                var refId = Guid.NewGuid();

                events[i] = new VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens(
                    refId,
                    oldEvent.VertegenwoordigerId,
                    oldEvent.IsPrimair
                );

                var vertegenwoordigerPersoonsgegevens = new VertegenwoordigerPersoonsgegevens(
                    refId,
                    VCode.Hydrate(aggregateId),
                    oldEvent.VertegenwoordigerId,
                    Insz.Hydrate(oldEvent.Insz),
                    oldEvent.Roepnaam,
                    oldEvent.Rol,
                    oldEvent.Voornaam,
                    oldEvent.Achternaam,
                    oldEvent.Email,
                    oldEvent.Telefoon,
                    oldEvent.Mobiel,
                    oldEvent.SocialMedia);

                await _vertegenwoordigerPersoonsgegevensRepository.Save(vertegenwoordigerPersoonsgegevens);
            }
        }
    }

    private static void TryLockForKboNumber(string vCode, IDocumentSession _session, IEvent? registreerEvent)
    {
        if (registreerEvent is VerenigingMetRechtspersoonlijkheidWerdGeregistreerd evnt)
            _session.Events.StartStream<KboNummer>(evnt.KboNummer, new KboNummerWerdGereserveerd(vCode));
    }

    private static StreamAction AppendEvents(
        IDocumentSession _session,
        string aggregateId,
        IReadOnlyCollection<IEvent> events,
        long? expectedVersion)
    {
        if (expectedVersion is not null)
            return _session.Events.Append(aggregateId, expectedVersion.Value + events.Count, events);

        return _session.Events.Append(aggregateId, events);
    }

    private static void SetHeaders(CommandMetadata metadata, IDocumentSession _session)
    {
        _session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        _session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        _session.CorrelationId = metadata.CorrelationId.ToString();
    }

    public async Task<T> Load<T>(string id, long? expectedVersion) where T : class, IHasVersion, new()
    {
        var aggregate = await _session.Events.AggregateStreamAsync<T>(id) ??
                        throw new AggregateNotFoundException(id, typeof(T));

        if (expectedVersion is not null && aggregate.Version != expectedVersion)
        {
            Throw<UnexpectedAggregateVersionException>.If(expectedVersion > aggregate.Version);

            var eventsInNewVersion = (await _session.Events.FetchStreamAsync(id, aggregate.Version))
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
        var streamState = await _session.Events.FetchStreamStateAsync(vCode);

        return streamState != null;
    }

    public async Task<bool> Exists(KboNummer kboNummer)
    {
        var streamState = await _session.Events.FetchStreamStateAsync(kboNummer);

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            await _session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                          .Where(x => x.KboNummer == kboNummer.Value)
                          .SingleOrDefaultAsync();

        return streamState != null ||
               verenigingMetRechtspersoonlijkheidWerdGeregistreerd != null;
    }

    public async Task<VCode?> GetVCodeForKbo(string kboNummer)
    {
        var id = (await _session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                .Where(geregistreerd => geregistreerd.KboNummer == kboNummer)
                                .SingleOrDefaultAsync())?.VCode;

        if (string.IsNullOrEmpty(id))
            return null;

        return VCode.Hydrate(id);
    }
}
