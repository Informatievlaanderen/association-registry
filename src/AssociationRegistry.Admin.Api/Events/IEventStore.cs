namespace AssociationRegistry.Admin.Api.Events;

using System.Threading.Tasks;

public interface IEventStore
{
    Task Save(string aggregateId, params IEvent[] events);
}
