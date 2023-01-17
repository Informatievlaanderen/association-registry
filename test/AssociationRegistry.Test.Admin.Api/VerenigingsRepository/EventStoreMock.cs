namespace AssociationRegistry.Test.Admin.Api.VerenigingsRepository.When_saving_a_vereniging;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;

public class EventStoreMock : IEventStore
{
    public record Invocation(string AggregateId, IEvent[] Events);

    public readonly List<Invocation> Invocations = new();

    public async Task<long> Save(string aggregateId, CommandMetadata metadata, params IEvent[] events)
    {
        Invocations.Add(new Invocation(aggregateId, events));
        return await Task.FromResult(-1);
    }

    public async Task<T> Load<T>(string id) where T : class
        => throw new NotImplementedException();
}
