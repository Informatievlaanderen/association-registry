namespace AssociationRegistry.EventStore;

using Events;

public class EventConflictResolver
{
    public readonly IEventPreConflictResolutionStrategy[] _preStrategies;
    public readonly IEventPostConflictResolutionStrategy[] _postStrategies;

    public EventConflictResolver(
        IEnumerable<IEventPreConflictResolutionStrategy> preStrategies,
        IEnumerable<IEventPostConflictResolutionStrategy> postStrategies)
    {
        _preStrategies = preStrategies.ToArray();
        _postStrategies = postStrategies.ToArray();
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
