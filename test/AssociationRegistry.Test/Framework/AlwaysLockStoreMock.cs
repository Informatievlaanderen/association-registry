namespace AssociationRegistry.Test.Framework;

using EventStore;
using EventStore.Locks;
using Vereniging;

public class AlwaysLockStoreMock : ILockStore
{

    public async Task<KboLockDocument?> GetKboNummerLock(KboNummer kboNummer)
        => await Task.FromResult(new KboLockDocument
        {
            KboNummer = kboNummer,
        });

    public async Task SetKboNummerLock(KboNummer kboNummer)
        => await Task.CompletedTask;

    public async Task DeleteKboNummerLock(KboNummer kboNummer)
        => await Task.CompletedTask;

    public async Task CleanKboNummerLocks()
        => await Task.CompletedTask;
}
