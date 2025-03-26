namespace AssociationRegistry.EventStore;

using Framework;
using Marten;
using Vereniging;
using Vereniging.Exceptions;

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

        return await _eventStore.Save(vereniging.VCode, metadata, cancellationToken, events);
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

        return await _eventStore.Save(vereniging.VCode, session, metadata, cancellationToken, events);
    }

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion, bool allowVerwijderdeVereniging = false, bool allowDubbeleVereniging = false)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(vCode, expectedVersion);

        if (!allowVerwijderdeVereniging)
            verenigingState.ThrowIfVerwijderd();

        if (!allowDubbeleVereniging)
            verenigingState.ThrowIfDubbel();

        var vereniging = new TVereniging();
        vereniging.Hydrate(verenigingState);

        return vereniging;
    }

    public async Task<VerenigingMetRechtspersoonlijkheid> Load(KboNummer kboNummer, long? expectedVersion)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(kboNummer, expectedVersion);
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
