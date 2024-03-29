namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using Marten.Events;
using Wolverine;
using Wolverine.Runtime.Routing;
using IEvent = Marten.Events.IEvent;

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

public class EventEnvelope<T> : IEventEnvelope
{
    public string VCode
        => Event.StreamKey!;

    public T Data
        => (T)Event.Data;

    public Dictionary<string, object>? Headers
        => Event.Headers;

    public EventEnvelope(IEvent @event)
    {
        Event = @event;
    }

    private IEvent Event { get; }
}

public interface IEventEnvelope
{
}
