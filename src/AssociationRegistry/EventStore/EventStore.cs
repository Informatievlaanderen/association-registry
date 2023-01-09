namespace AssociationRegistry.EventStore;

using System.Linq;
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

    public async Task<long> Save(string aggregateId, CommandMetadata metadata, params IEvent[] events)
    {
        await using var session = _documentStore.OpenSession();

        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, metadata.Tijdstip);

        var streamAction = session.Events.Append(aggregateId, events.As<object[]>());

        await session.SaveChangesAsync();

        return streamAction.Events.Max(@event => @event.Sequence);
    }
}
