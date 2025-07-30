namespace AssociationRegistry.MartenDb.Subscriptions;

using JasperFx.Events;
using System.Collections.ObjectModel;

public class SubscriptionEventList
{
    public IReadOnlyList<IEvent> Events { get; }
    public Dictionary<string, IEvent[]> GroupedByVCode { get; set; }

    private SubscriptionEventList(IReadOnlyList<IEvent> events)
    {
        Events = new ReadOnlyCollection<IEvent>(events.ExcludeTombstones().ToList());

        GroupedByVCode = events
                        .GroupBy(x => x.StreamKey)
                        .ToDictionary(x => x.Key!, x => x.ToArray());
    }

    public static SubscriptionEventList From(IReadOnlyList<IEvent> events)
    {
        return new SubscriptionEventList(events);
    }
}
