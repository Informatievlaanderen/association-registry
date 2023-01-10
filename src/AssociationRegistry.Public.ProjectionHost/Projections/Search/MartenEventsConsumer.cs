namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten.Events;
using Wolverine;

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
            var eventEnvelope = (IEventEnvelope)Activator.CreateInstance(typeof(EventEnvelope<> ).MakeGenericType(@event.EventType), @event)!;
            await _bus.PublishAsync(eventEnvelope);
        }
    }
}

public class EventEnvelope<T>: IEventEnvelope
{
    public T Data => (T)Event.Data;

    public Dictionary<string, object>? Headers
        => Event.Headers;

    public EventEnvelope(IEvent @event)
        => Event = @event;

    private IEvent Event { get; }
}

public interface IEventEnvelope {
}
