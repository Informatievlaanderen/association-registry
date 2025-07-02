namespace AssociationRegistry.EventStore;

using Events;

public interface IEventPostConflictResolutionStrategy
{
    bool IsAllowedConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<JasperFx.Events.IEvent> conflictingEvents);
}

public interface IEventPreConflictResolutionStrategy
{
    bool IsAllowedConflict(IEnumerable<JasperFx.Events.IEvent> conflictingEvents);
}
