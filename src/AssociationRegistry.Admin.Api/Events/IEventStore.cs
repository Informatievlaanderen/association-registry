namespace AssociationRegistry.Admin.Api.Events;

using System.Threading.Tasks;
using Framework;

public interface IEventStore
{
    Task Save(string aggregateId, params IEvent[] events);
}
