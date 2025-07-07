namespace AssociationRegistry.EventStore;

using Events;

public class EventConflictResolver
{
    private readonly IEventPreConflictResolutionStrategy[] _preStrategies;
    private readonly IEventPostConflictResolutionStrategy[] _postStrategies;

    public EventConflictResolver()
    {
        _preStrategies = Array.Empty<IEventPreConflictResolutionStrategy>();
        _postStrategies = Array.Empty<IEventPostConflictResolutionStrategy>();
    }

    public EventConflictResolver(IEventPreConflictResolutionStrategy[] preStrategies, IEventPostConflictResolutionStrategy[] postStrategies)
    {
        _preStrategies = preStrategies;
        _postStrategies = postStrategies;
    }

    public bool IsAllowedPostConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<JasperFx.Events.IEvent> conflictingEvents)
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

    public bool IsAllowedPreConflict(IEnumerable<JasperFx.Events.IEvent> conflictingEvents)
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
