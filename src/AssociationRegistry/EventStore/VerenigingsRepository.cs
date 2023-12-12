namespace AssociationRegistry.EventStore;

using Framework;
using Locks;
using Marten;
using Vereniging;

public class VerenigingsRepository : IVerenigingsRepository
{
    private readonly IEventStore _eventStore;
    private readonly ILockStore _lockStore;

    public VerenigingsRepository(IEventStore eventStore, ILockStore lockStore)
    {
        _eventStore = eventStore;
        _lockStore = lockStore;
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

    public async Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion)
        where TVereniging : IHydrate<VerenigingState>, new()
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(vCode);

        var vereniging = new TVereniging();
        vereniging.Hydrate(verenigingState);

        if (expectedVersion is not null && verenigingState.Version != expectedVersion)
            throw new UnexpectedAggregateVersionException();

        return vereniging;
    }

    public async Task<VCodeAndNaam?> GetVCodeAndNaam(KboNummer kboNummer)
    {
        var verenigingState = await _eventStore.Load<VerenigingState>(kboNummer);

        if (verenigingState == null)
            return null;

        return new VCodeAndNaam(verenigingState.VCode, verenigingState.Naam);
    }

    public async Task<KboLockDocument?> GetKboNummerLock(KboNummer kboNummer)
    {
        await _lockStore.CleanKboNummerLocks();

        return await _lockStore.GetKboNummerLock(kboNummer);
    }

    public async Task SetKboNummerLock(KboNummer kboNummer)
        => await _lockStore.SetKboNummerLock(kboNummer);

    public async Task DeleteKboNummerLock(KboNummer kboNummer)
        => await _lockStore.DeleteKboNummerLock(kboNummer);

    public record VCodeAndNaam(VCode? VCode, VerenigingsNaam VerenigingsNaam)
    {
        public static VCodeAndNaam Fallback(KboNummer kboNummer)
            => new(
                VCode: null,
                VerenigingsNaam.Create($"Moeder {kboNummer}"));
    }
}
