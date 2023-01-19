namespace AssociationRegistry.Test.Admin.Api.VerenigingsRepository;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;

public class EventStoreMock : IEventStore
{
    public record Invocation(string AggregateId, IEvent[] Events);

    public readonly List<Invocation> Invocations = new();

    public async Task<SaveChangesResult> Save(string aggregateId, CommandMetadata metadata, params IEvent[] events)
    {
        Invocations.Add(new Invocation(aggregateId, events));
        return await Task.FromResult(new SaveChangesResult(-1, -1));
    }

    public async Task<T> Load<T>(string id, long? expectedVersion) where T : class, IHasVersion
        => throw new NotImplementedException();
}
