namespace AssociationRegistry.MartenDb.Subscriptions;

using JasperFx.Events;

public static class EventListExtensions
{
    public static IReadOnlyList<IEvent> ExcludeTombstones(this IReadOnlyList<IEvent> events)
    {
        return events.Where(x => x.EventType != typeof(Tombstone)).ToList();
    }
}
