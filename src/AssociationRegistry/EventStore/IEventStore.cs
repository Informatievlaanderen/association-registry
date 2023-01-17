namespace AssociationRegistry.EventStore;

using System.Threading.Tasks;
using Framework;
using IEvent = Framework.IEvent;

public interface IEventStore
{
    Task<long> Save(string aggregateId, CommandMetadata commandMetadata, params IEvent[] events);
    // Task<IEventStream<Vereniging>> Load(string vCode);
    Task<T> Load<T>(string id) where T : class;
}
