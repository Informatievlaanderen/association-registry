namespace AssociationRegistry.EventStore;

using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Framework;
using JasperFx.Core.Reflection;
using Marten;
using Marten.Events;
using VCodes;
using Vereniging;
using IEvent = Framework.IEvent;

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

    public async Task<T> Load<T>(string id) where T : class
    {
        await using var session = _documentStore.OpenSession();

        return await session.Events.AggregateStreamAsync<T>(id) ??
               throw new AggregateNotFoundException(id, typeof(T));
    }

    // public async Task<IEventStream<Vereniging>> Load(IDocumentSession session, string vCode)
    // {
    //     return await session.Events.FetchForWriting<Vereniging>(vCode);
    // }
}
