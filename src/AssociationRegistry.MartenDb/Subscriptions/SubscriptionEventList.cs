namespace AssociationRegistry.MartenDb.Subscriptions;

using JasperFx.Events;
using System.Collections.ObjectModel;

public class SubscriptionEventList
{
    public IReadOnlyList<IEvent> Events { get; }
    public Dictionary<string, IEvent[]> GroupedByVCode { get; set; }

    private SubscriptionEventList(IReadOnlyList<IEvent> events, Type[] handledEventTypes)
    {
        var onlyHandledEvents = events
                               .ExcludeTombstones()
                               .Where(x => handledEventTypes.Contains(x.EventType))
                               .ToList();

        Events = new ReadOnlyCollection<IEvent>(onlyHandledEvents);

        GroupedByVCode = Events
                        .GroupBy(x => x.StreamKey)
                        .ToDictionary(x => x.Key!, x => x.ToArray());
    }

    public static SubscriptionEventList From(IReadOnlyList<IEvent> events, Type[] handledEventTypes)
        => new(events, handledEventTypes);
}
