namespace AssociationRegistry.Public.Api.Projections;

using System.Threading.Tasks;
using Events;

public interface IDomainEventHandler<in T>
    where T : class, IEvent
{
    Task HandleEvent(T message);
}
