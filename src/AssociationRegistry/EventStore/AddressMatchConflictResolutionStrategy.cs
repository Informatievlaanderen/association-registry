﻿namespace AssociationRegistry.EventStore;

using Events;
using Framework;

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
            typeof(AdresWerdGewijzigdInAdressenregister)
        }.Contains(eventType);


}
