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
using Transformers;
using IEvent = Events.IEvent;

public class EventStore : IEventStore
{
    public class ExpectedVersion
    {
        public const long NewStream = 0;
    }

    private readonly IDocumentSession _session;
    private readonly EventConflictResolver _conflictResolver;
    private readonly IPersoonsgegevensProcessor _persoonsgegevensProcessor;
    private readonly ILogger<EventStore> _logger;

    public EventStore(
        IDocumentSession session,
        EventConflictResolver conflictResolver,
        IPersoonsgegevensProcessor persoonsgegevensProcessor,
        ILogger<EventStore> logger
    )
    {
        _session = session;
        _conflictResolver = conflictResolver;
        _persoonsgegevensProcessor = persoonsgegevensProcessor;
        _logger = logger;
    }

    public async Task<StreamActionResult> SaveNew(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events
    )
    {
        var processedEvents = await _persoonsgegevensProcessor.ProcessEvents(aggregateId: aggregateId, events: events);

        SetMetadata(metadata);

        var streamAction = _session.Events.StartStream(streamKey: aggregateId, events: processedEvents);

        await _session.SaveChangesAsync(cancellationToken);

        var maxSequence = streamAction.Events.Max(@event => @event.Sequence);

        if (maxSequence == 0)
            maxSequence = (
                await _session.Events.FetchStreamAsync(streamKey: aggregateId, token: cancellationToken)
            ).Max(x => x.Sequence);

        return new StreamActionResult(Sequence: maxSequence, Version: processedEvents.Length);
    }

    public async Task<StreamActionResult> SaveNewKbo(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        IEvent[] events
    )
    {
        var processedEvents = await _persoonsgegevensProcessor.ProcessEvents(aggregateId: aggregateId, events: events);

        SetMetadata(metadata);

        ClaimKboNummer(vCode: aggregateId, processedEvents.FirstOrDefault());

        var streamAction = _session.Events.StartStream(streamKey: aggregateId, events: processedEvents);

        await _session.SaveChangesAsync(cancellationToken);

        var maxSequence = streamAction.Events.Max(@event => @event.Sequence);

        if (maxSequence == 0)
            maxSequence = (
                await _session.Events.FetchStreamAsync(streamKey: aggregateId, token: cancellationToken)
            ).Max(x => x.Sequence);

        return new StreamActionResult(Sequence: maxSequence, Version: processedEvents.Length);
    }

    public async Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events
    )
    {
        var processedEvents = await _persoonsgegevensProcessor.ProcessEvents(aggregateId: aggregateId, events: events);

        try
        {
            SetMetadata(metadata);

            var streamAction = AppendEvents(
                aggregateId: aggregateId,
                events: processedEvents,
                expectedVersion: aggregateVersion
            );

            await _session.SaveChangesAsync(cancellationToken);

            var eventsAgain = await _session.Events.FetchStreamAsync(streamKey: aggregateId, token: cancellationToken);
            var maxSequence = eventsAgain.Max(@event => @event.Sequence);

            _logger.LogInformation(
                message: "SAVED EVENTS {@EventNames} with max sequence: {MaxSeq}",
                string.Join(separator: ", ", processedEvents.Select(x => x.GetType().Name)),
                maxSequence
            );

            return new StreamActionResult(eventsAgain.Max(@event => @event.Sequence), eventsAgain.Max(x => x.Version));
        }
        catch (EventStreamUnexpectedMaxEventIdException)
        {
            var eventsDiff = await _session.Events.FetchStreamAsync(
                streamKey: aggregateId,
                fromVersion: aggregateVersion + 1,
                token: cancellationToken
            );

            if (_conflictResolver.IsAllowedPostConflict(intendedEvents: processedEvents, conflictingEvents: eventsDiff))
            {
                _session.EjectAllPendingChanges();

                processedEvents = await _persoonsgegevensProcessor.ProcessEvents(
                    aggregateId: aggregateId,
                    events: events
                );

                var streamAction = _session.Events.Append(
                    stream: aggregateId,
                    aggregateVersion + processedEvents.Length + eventsDiff.Count,
                    events: processedEvents
                );

                await _session.SaveChangesAsync(cancellationToken);

                var eventsAgain = await _session.Events.FetchStreamAsync(
                    streamKey: aggregateId,
                    token: cancellationToken
                );

                var maxSequence = eventsAgain.Max(@event => @event.Sequence);

                _logger.LogInformation(
                    message: "SAVED EVENTS {@EventNames} with max sequence: {MaxSeq}",
                    string.Join(separator: ", ", processedEvents.Select(x => x.GetType().Name)),
                    maxSequence
                );

                return new StreamActionResult(
                    eventsAgain.Max(@event => @event.Sequence),
                    eventsAgain.Max(x => x.Version)
                );
            }

            throw new UnexpectedAggregateVersionException();
        }
    }

    public void ClaimKboNummer(string vCode, IEvent? registreerEvent)
    {
        if (registreerEvent is VerenigingMetRechtspersoonlijkheidWerdGeregistreerd evnt)
            _session.Events.StartStream<KboNummer>(streamKey: evnt.KboNummer, new KboNummerWerdGereserveerd(vCode));
    }

    private StreamAction AppendEvents(string aggregateId, IReadOnlyCollection<IEvent> events, long? expectedVersion)
    {
        if (expectedVersion is not null)
            return _session.Events.Append(stream: aggregateId, expectedVersion.Value + events.Count, events: events);

        return _session.Events.Append(stream: aggregateId, events: events);
    }

    public void SetMetadata(CommandMetadata metadata)
    {
        _session.SetHeader(key: MetadataHeaderNames.Initiator, value: metadata.Initiator);
        _session.SetHeader(key: MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        _session.CorrelationId = metadata.CorrelationId.ToString();

        if (metadata.AdditionalMetadata != null)
            foreach (var item in metadata.AdditionalMetadata.Items)
            {
                item.ApplyTo(_session);
            }
    }

    public async Task<T> Load<T>(string id, long? expectedVersion)
        where T : class, IHasVersion, new()
    {
        var aggregate =
            await _session.Events.AggregateStreamAsync<T>(id)
            ?? throw new AggregateNotFoundException(identifier: id, typeof(T));

        if (expectedVersion is not null && aggregate.Version != expectedVersion)
        {
            Throw<UnexpectedAggregateVersionException>.If(expectedVersion > aggregate.Version);

            var eventsInNewVersion = (
                await _session.Events.FetchStreamAsync(streamKey: id, version: aggregate.Version)
            ).Where(x => x.Version > expectedVersion);

            if (!_conflictResolver.IsAllowedPreConflict(eventsInNewVersion))
                throw new UnexpectedAggregateVersionException();
        }

        return aggregate;
    }

    public async Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion)
        where T : class, IHasVersion, new()
    {
        var vCode = await GetVCodeForKbo(kboNummer);

        if (vCode is null)
            throw new AggregateNotFoundException(identifier: kboNummer, typeof(T));

        return await Load<T>(vCode!, expectedVersion: expectedVersion);
    }

    public async Task<bool> Exists(VCode vCode)
    {
        var streamState = await _session.Events.FetchStreamStateAsync(vCode);

        return streamState != null;
    }

    public async Task<VCode?> GetVCodeForKbo(string kboNummer)
    {
        var id = (
            await _session
                .Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                .Where(geregistreerd => geregistreerd.KboNummer == kboNummer)
                .SingleOrDefaultAsync()
        )?.VCode;

        if (string.IsNullOrEmpty(id))
            return null;

        return VCode.Hydrate(id);
    }
}
