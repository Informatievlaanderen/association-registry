namespace AssociationRegistry.Vereniging;

using EventStore;
using EventStore.Locks;
using Framework;

public interface IVerenigingsRepository
{
    Task<StreamActionResult> Save(VerenigingsBase vereniging, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion) where TVereniging : IHydrate<VerenigingState>, new();
    Task<VerenigingsRepository.VCodeAndNaam?> GetVCodeAndNaam(KboNummer kboNummer);
    Task<KboLockDocument?> GetKboNummerLock(KboNummer kboNummer);
    Task SetKboNummerLock(KboNummer kboNummer);
    Task DeleteKboNummerLock(KboNummer kboNummer);
}
