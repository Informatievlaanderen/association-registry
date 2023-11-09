namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using Marten.Events;
using Wolverine;
using Wolverine.Runtime.Routing;

public class MartenEventsConsumer : IMartenEventsConsumer
{
    private readonly IMessageBus _bus;

    public MartenEventsConsumer(IMessageBus bus)
    {
        _bus = bus;
    }

    public async Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions)
    {
        foreach (var @event in streamActions.SelectMany(streamAction => streamAction.Events))
        {
            var eventEnvelope =
                (IEventEnvelope)Activator.CreateInstance(typeof(EventEnvelope<>).MakeGenericType(@event.EventType), @event)!;

            try
            {
                await _bus.InvokeAsync(eventEnvelope);
            }
            catch (IndeterminateRoutesException)
            {
                //ignore
            }
        }
    }
}
