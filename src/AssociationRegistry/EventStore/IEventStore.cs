namespace AssociationRegistry.EventStore;

using System.Threading.Tasks;
using Framework;
using IEvent = Framework.IEvent;

public interface IEventStore
{
    Task<SaveChangesResult> Save(string aggregateId, CommandMetadata commandMetadata, params IEvent[] events);

    Task<T> Load<T>(string id, long? expectedVersion) where T : class, IHasVersion;
}
