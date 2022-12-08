namespace AssociationRegistry.Admin.Api.Events;

using System.Threading.Tasks;
using Baseline;
using Framework;
using Marten;

public class EventStore : IEventStore
{
    private readonly IDocumentStore _documentStore;

    public EventStore(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task Save(string aggregateId, CommandMetadata metadata, params IEvent[] events)
    {
        await using var session = _documentStore.OpenSession();

        session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        session.SetHeader(MetadataHeaderNames.Tijdstip, metadata.Tijdstip);

        session.Events.Append(aggregateId, events.As<object[]>());

        await session.SaveChangesAsync();
    }
}
