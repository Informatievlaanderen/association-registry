namespace AssociationRegistry.EventStore;

using Events;

public class EmptyConflictResolutionStrategy : IEventPostConflictResolutionStrategy, IEventPreConflictResolutionStrategy
{
    public bool IsAllowedConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<JasperFx.Events.IEvent> conflictingEvents)
        => false;

    public bool IsAllowedConflict(IEnumerable<JasperFx.Events.IEvent> conflictingEvents)
        => false;
}
