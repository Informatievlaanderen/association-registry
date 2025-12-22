namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using Events;
using Marten;
using OpenTelemetry.Metrics;
using Persoonsgegevens;

public class VerenigingsRepository : IVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public VerenigingsRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(vereniging.VCode, vereniging.Version, metadata, cancellationToken, events);
    }

    public async Task<StreamActionResult> Save(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(vereniging.VCode, vereniging.Version, metadata, cancellationToken, events);
    }

    public async Task<StreamActionResult> SaveNew(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.SaveNew(vereniging.VCode, metadata, cancellationToken, events);
    }

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, CommandMetadata metadata, bool allowVerwijderdeVereniging = false, bool allowDubbeleVereniging = false)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        RepositoryMetrics.RecordAggregateLoaded(metadata.ExpectedVersion.HasValue, metadata.Initiator);

        var verenigingState = await _eventStore.Load<VerenigingState>(vCode, metadata.ExpectedVersion);

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
        RepositoryMetrics.RecordAggregateLoaded(metadata.ExpectedVersion.HasValue, metadata.Initiator);

        var verenigingState = await _eventStore.Load<VerenigingState>(kboNummer, metadata.ExpectedVersion);
        var verenigingMetRechtspersoonlijkheid = new VerenigingMetRechtspersoonlijkheid();
        verenigingMetRechtspersoonlijkheid.Hydrate(verenigingState);

        return verenigingMetRechtspersoonlijkheid;
    }

    public async Task<bool> IsVerwijderd(VCode vCode)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(vCode, null);

        return verenigingState.IsVerwijderd;
    }

    public async Task<bool> IsDubbel(VCode vCode)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(vCode, null);

        return verenigingState.VerenigingStatus is VerenigingStatus.StatusDubbel;
    }

    public async Task<bool> Exists(VCode vCode)
        => await _eventStore.Exists(vCode);

    public async Task<bool> Exists(KboNummer kboNummer)
        => await _eventStore.Exists(kboNummer);

}
