namespace AssociationRegistry.Admin.Api.Events;

using System.Threading.Tasks;
using Framework;
using JasperFx.Core.Reflection;
using Marten;

public class EventStore : IEventStore
{
    private readonly IDocumentStore _documentStore;

    public EventStore(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task Save(string aggregateId, CommandMetadata commandMetadata, params IEvent[] events)
    {
        await using var session = _documentStore.OpenSession();

        session.Events.Append(aggregateId, events.As<object[]>());

        await session.SaveChangesAsync();
    }
}
