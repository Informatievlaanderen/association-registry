namespace AssociationRegistry.EventStore;

using Framework;

public class EventConflictResolver
{
    private readonly IEventPostConflictResolutionStrategy[] _postStrategies;
    private readonly IEventPreConflictResolutionStrategy[] _preStrategies;

    public EventConflictResolver(IEventPreConflictResolutionStrategy[] preStrategies, IEventPostConflictResolutionStrategy[] postStrategies)
    {
        _postStrategies = postStrategies;
        _preStrategies = preStrategies;
    }

    public bool IsAllowedPostConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<Marten.Events.IEvent> conflictingEvents)
    {
        foreach (var strategy in _postStrategies)
        {
            if (strategy.IsAllowedConflict(intendedEvents, conflictingEvents))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsAllowedPreConflict(IEnumerable<Marten.Events.IEvent> conflictingEvents)
    {
        foreach (var strategy in _preStrategies)
        {
            if (strategy.IsAllowedConflict(conflictingEvents))
            {
                return true;
            }
        }

        return false;
    }
}
