namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Marten;
using OpenTelemetry.Metrics;
using Persoonsgegevens;
using Transformers;
using Vereniging;

public class AggregateSession : IAggregateSession

{
    private readonly IEventStore _eventStore;

    public AggregateSession(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default
    )
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(
            aggregateId: vereniging.VCode,
            aggregateVersion: vereniging.Version,
            metadata: metadata,
            cancellationToken: cancellationToken,
            events: events
        );
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    )
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(
            aggregateId: vereniging.VCode,
            aggregateVersion: vereniging.Version,
            metadata: metadata,
            cancellationToken: cancellationToken,
            events: events
        );
    }

    public async Task<TVereniging> Load<TVereniging>(
        VCode vCode,
        CommandMetadata metadata,
        bool allowVerwijderdeVereniging = false,
        bool allowDubbeleVereniging = false
    )
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        RepositoryMetrics.RecordAggregateLoaded(
            hasExpectedVersion: metadata.ExpectedVersion.HasValue,
            initiator: metadata.Initiator
        );

        var verenigingState = await _eventStore.Load<VerenigingState>(
            id: vCode,
            expectedVersion: metadata.ExpectedVersion
        );

        if (!allowVerwijderdeVereniging)
            verenigingState.ThrowIfVerwijderd();

        if (!allowDubbeleVereniging)
            verenigingState.ThrowIfDubbel();

        var vereniging = new TVereniging();
        vereniging.Hydrate(verenigingState);

        return vereniging;
    }

    public async Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata)
    {
        RepositoryMetrics.RecordAggregateLoaded(
            hasExpectedVersion: metadata.ExpectedVersion.HasValue,
            initiator: metadata.Initiator
        );

        var verenigingState = await _eventStore.Load<VerenigingState>(
            kboNummer: kboNummer,
            expectedVersion: metadata.ExpectedVersion
        );

        var verenigingMetRechtspersoonlijkheid = new VerenigingMetRechtspersoonlijkheid();
        verenigingMetRechtspersoonlijkheid.Hydrate(verenigingState);

        return verenigingMetRechtspersoonlijkheid;
    }
}
