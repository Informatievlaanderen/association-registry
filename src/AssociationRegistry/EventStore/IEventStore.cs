namespace AssociationRegistry.EventStore;

using System.Threading.Tasks;
using Framework;

public interface IEventStore
{
    Task<long> Save(string aggregateId, CommandMetadata commandMetadata, params IEvent[] events);
}
