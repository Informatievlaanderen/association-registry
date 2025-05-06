namespace AssociationRegistry.Test.Framework;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using Events;
using EventStore;
using Marten;
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
        long aggregateVersion,
        CommandMetadata metadata,
        CancellationToken cancellationToken = default,
        params IEvent[] events)
    {
        SaveInvocations.Add(new SaveInvocation(aggregateId, events));

        return Task.FromResult(new StreamActionResult(Sequence: -1, Version: -1));
    }

    public async Task<StreamActionResult> Save(
        string aggregateId,
        long aggregateVersion,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken,
        params IEvent[] events)
        => await Save(aggregateId, 0, metadata, cancellationToken, events);

    public Task<T> Load<T>(string aggregateId, long? expectedVersion) where T : class, IHasVersion, new()
    {
        var result = new T();

        for (var i = 0; i < _events.Length; i++)
        {
            result = ((dynamic)result).Apply((dynamic)_events[i]);
            result.Version = i + 1;
        }

        return Task.FromResult(result);
    }

    public Task<T?> Load<T>(KboNummer kboNummer, long? expectedVersion) where T : class, IHasVersion, new()
        => Task.FromException<T?>(new NotImplementedException());

    public async Task<bool> Exists(VCode vCode)
        => true;

    public async Task<bool> Exists(KboNummer kboNummer)
        => throw new NotImplementedException();
}
