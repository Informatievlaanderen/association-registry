namespace AssociationRegistry.Test.Framework;

using EventStore;
using AssociationRegistry.Framework;
using EventStore.Locks;
using Vereniging;

public class EventStoreMock : IEventStore
{
    public record SaveInvocation(string AggregateId, IEvent[] Events);
    public record LoadInvocation(string AggregateId, Type Type);
    private readonly IEvent[] _events;

    public EventStoreMock(params IEvent[] events)
    {
        _events = events;
    }

    public readonly List<SaveInvocation> SaveInvocations = new();

    public Task<StreamActionResult> Save(
        string aggregateId,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default,
        params IEvent[] events)
    {
        SaveInvocations.Add(new SaveInvocation(aggregateId, events));

        return Task.FromResult(new StreamActionResult(-1, -1));
    }

    public Task<T> Load<T>(string aggregateId) where T : class, IHasVersion, new()
    {
        var result = new T();

        for (var i = 0; i < _events.Length; i++)
        {
            result = ((dynamic)result).Apply((dynamic)_events[i]);
            result.Version = i + 1;
        }

        return Task.FromResult(result);
    }

    public Task<T?> Load<T>(KboNummer kboNummer) where T : class, IHasVersion, new()
        => Task.FromException<T?>(new NotImplementedException());
}

public class LockStoreMock : ILockStore
{
    private List<KboLockDocument> docs = new();

    public async Task<KboLockDocument?> GetKboNummerLock(KboNummer kboNummer)
        => docs.SingleOrDefault(d => d.KboNummer == kboNummer);

    public async Task SetKboNummerLock(KboNummer kboNummer)
        => docs.Add(new KboLockDocument
        {
            KboNummer = kboNummer,
            CreatedAt = DateTimeOffset.UtcNow,
        });

    public async Task DeleteKboNummerLock(KboNummer kboNummer)
        => docs = docs.Where(d => d.KboNummer != kboNummer).ToList();

    public async Task CleanKboNummerLocks()
        => docs = docs.Where(d => d.CreatedAt <= DateTimeOffset.UtcNow.AddMinutes(-1)).ToList();
}
