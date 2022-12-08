namespace AssociationRegistry.Admin.Api.Events;

using System.Threading.Tasks;
using Framework;

public interface IEventStore
{
    Task Save(string aggregateId, CommandMetadata commandMetadata, params IEvent[] events);
}
