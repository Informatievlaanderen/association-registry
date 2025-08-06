namespace AssociationRegistry.EventStore.ConflictResolution;

using Events;

public class AddressMatchConflictResolutionStrategy : IEventPostConflictResolutionStrategy, IEventPreConflictResolutionStrategy
{
    public bool IsAllowedConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<JasperFx.Events.IEvent> conflictingEvents)
        => conflictingEvents.All(e => IsAllowedEventType(e.EventType));

    public bool IsAllowedConflict(IEnumerable<JasperFx.Events.IEvent> conflictingEvents)
        => conflictingEvents.All(e => IsAllowedEventType(e.EventType));

    private bool IsAllowedEventType(Type eventType)
        => AllowedTypes.Contains(eventType);

    public static Type[] AllowedTypes =>
    [
        typeof(AdresWerdOvergenomenUitAdressenregister),
        typeof(AdresKonNietOvergenomenWordenUitAdressenregister),
        typeof(AdresWerdNietGevondenInAdressenregister),
        typeof(AdresNietUniekInAdressenregister),
        typeof(AdresWerdGewijzigdInAdressenregister),
        typeof(AdresWerdOntkoppeldVanAdressenregister),
        typeof(LocatieDuplicaatWerdVerwijderdNaAdresMatch),
        typeof(AdresHeeftGeenVerschillenMetAdressenregister),
    ];
}
