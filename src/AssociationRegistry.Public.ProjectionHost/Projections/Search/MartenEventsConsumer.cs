namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;

public class MartenEventsConsumer : IMartenEventsConsumer
{
    private readonly IServiceProvider _serviceProvider;

    public MartenEventsConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions)
    {
        foreach (var @event in streamActions.SelectMany(streamAction => streamAction.Events))
        {
            if (!@event.EventType.IsAssignableTo(typeof(AssociationRegistry.Framework.IEvent)))
                return;

            var eventHandlers = _serviceProvider.GetServices(typeof(IDomainEventHandler<>).MakeGenericType(@event.EventType));

            foreach (var eventHandler in eventHandlers)
            {
                var handlerMethod = eventHandler!.GetType().GetMethod("HandleEvent", new[] { @event.EventType });
                await (Task) handlerMethod!.Invoke((dynamic?)eventHandler, new[]{@event.Data});
            }
        }
    }
}
