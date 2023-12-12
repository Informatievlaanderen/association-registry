namespace AssociationRegistry.Test.Framework;

using EventStore;
using EventStore.Locks;
using Vereniging;

public class CountedLockStoreMock : ILockStore
{
    private readonly int _retryCount;
    private int _callCounter;

    public CountedLockStoreMock(int retryCount = 1)
    {
        _retryCount = retryCount;
        _callCounter = 0;
    }

    public async Task<KboLockDocument?> GetKboNummerLock(KboNummer kboNummer)
    {
        _callCounter++;

        return await Task.FromResult(_callCounter > _retryCount
                                         ? null
                                         : new KboLockDocument
                                         {
                                             KboNummer = kboNummer,
                                         });
    }

    public async Task SetKboNummerLock(KboNummer kboNummer)
        => await Task.CompletedTask;

    public async Task DeleteKboNummerLock(KboNummer kboNummer)
        => await Task.CompletedTask;

    public async Task CleanKboNummerLocks()
        => await Task.CompletedTask;
}
