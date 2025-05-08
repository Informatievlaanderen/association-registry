namespace AssociationRegistry.EventStore;

using Events;

public class AddressMatchConflictResolutionStrategy : IEventPostConflictResolutionStrategy, IEventPreConflictResolutionStrategy
{
    public bool IsAllowedConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<Marten.Events.IEvent> conflictingEvents)
        => conflictingEvents.All(e => IsAllowedEventType(e.EventType));

    public bool IsAllowedConflict(IEnumerable<Marten.Events.IEvent> conflictingEvents)
        => conflictingEvents.All(e => IsAllowedEventType(e.EventType));

    private bool IsAllowedEventType(Type eventType)
        => new[]
        {
            typeof(AdresWerdOvergenomenUitAdressenregister),
            typeof(AdresKonNietOvergenomenWordenUitAdressenregister),
            typeof(AdresWerdNietGevondenInAdressenregister),
            typeof(AdresNietUniekInAdressenregister),
            typeof(AdresWerdGewijzigdInAdressenregister),
            typeof(AdresWerdOntkoppeldVanAdressenregister),
            typeof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
            typeof(AdresHeeftGeenVerschillenMetAdressenregister),
        }.Contains(eventType);
}
