namespace AssociationRegistry.EventStore;

using Locks;
using Vereniging;

public interface ILockStore
{
    Task<KboLockDocument?> GetKboNummerLock(KboNummer kboNummer);
    Task SetKboNummerLock(KboNummer kboNummer);
    Task DeleteKboNummerLock(KboNummer kboNummer);
    Task CleanKboNummerLocks();
}
