namespace AssociationRegistry.EventStore;

using Events;

public interface IEventPostConflictResolutionStrategy
{
    bool IsAllowedConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<Marten.Events.IEvent> conflictingEvents);
}

public interface IEventPreConflictResolutionStrategy
{
    bool IsAllowedConflict(IEnumerable<Marten.Events.IEvent> conflictingEvents);
}
