namespace AssociationRegistry.Test.Framework;

using EventStore;
using AssociationRegistry.Framework;
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

    public Task<StreamActionResult> Save(string aggregateId, CommandMetadata metadata, CancellationToken cancellationToken = default, params IEvent[] events)
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
            result.Version = i+1;
        }

        return Task.FromResult(result);
    }

    public async Task<T?> Load<T>(KboNummer kboNummer) where T : class, IHasVersion, new()
        => throw new NotImplementedException();
}
