namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Marten;
using OpenTelemetry.Metrics;
using Persoonsgegevens;
using Vereniging;

public interface IAggregateSession
{
    Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default
    );

    Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );

    Task<TVereniging> Load<TVereniging>(
        VCode vCode,
        CommandMetadata metadata,
        bool allowVerwijderdeVereniging = false,
        bool allowDubbeleVereniging = false
    )
        where TVereniging : IHydrate<VerenigingState>, new();

    Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, CommandMetadata metadata);
}

public interface INewAggregateSession
{
    Task<StreamActionResult> SaveNew(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );
}

public class VerenigingsRepository : IAggregateSession, INewAggregateSession, IVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public VerenigingsRepository(IEventStore eventStore)
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

    public async Task<StreamActionResult> SaveNew(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    )
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.SaveNew(
            aggregateId: vereniging.VCode,
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

    public async Task<bool> IsVerwijderd(VCode vCode)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(id: vCode, expectedVersion: null);

        return verenigingState.IsVerwijderd;
    }

    public async Task<bool> IsDubbel(VCode vCode)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(id: vCode, expectedVersion: null);

        return verenigingState.VerenigingStatus is VerenigingStatus.StatusDubbel;
    }

    public async Task<bool> Exists(VCode vCode) => await _eventStore.Exists(vCode);

    public async Task<bool> Exists(KboNummer kboNummer) => await _eventStore.Exists(kboNummer);
}
