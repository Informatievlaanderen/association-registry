namespace AssociationRegistry.EventStore;

using Framework;

public interface IEventConflictResolutionStrategy
{
    bool IsAllowedConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<Marten.Events.IEvent> conflictingEvents);
}
