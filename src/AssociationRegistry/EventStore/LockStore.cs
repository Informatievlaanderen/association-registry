namespace AssociationRegistry.EventStore;

using Locks;
using Marten;
using Vereniging;

public class LockStore : ILockStore
{
    private readonly IDocumentStore _documentStore;

    public LockStore(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task CleanKboNummerLocks()
    {
        await using var session = _documentStore.LightweightSession();
        session.DeleteWhere<KboLockDocument>(doc => doc.CreatedAt <= DateTimeOffset.UtcNow.AddMinutes(-1));
        await session.SaveChangesAsync();
    }

    public async Task<KboLockDocument?> GetKboNummerLock(KboNummer kboNummer)
    {
        try
        {
            await using var session = _documentStore.QuerySession();

            return await session.LoadAsync<KboLockDocument>(kboNummer);
        }
        catch
        {
            return await Task.FromResult<KboLockDocument?>(null);
        }
    }

    public async Task SetKboNummerLock(KboNummer kboNummer)
    {
        await using var session = _documentStore.LightweightSession();

        session.Store(new KboLockDocument
        {
            KboNummer = kboNummer,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await session.SaveChangesAsync();
    }

    public async Task DeleteKboNummerLock(KboNummer kboNummer)
    {
        await using var session = _documentStore.LightweightSession();

        session.Delete<KboLockDocument>(kboNummer);

        await session.SaveChangesAsync();
    }
}
