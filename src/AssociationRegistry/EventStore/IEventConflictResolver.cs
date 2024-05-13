namespace AssociationRegistry.EventStore;

using Framework;

public class EventConflictResolver
{
    private readonly IEventConflictResolutionStrategy[] _strategies;

    public EventConflictResolver(IEventConflictResolutionStrategy[] strategies)
    {
        _strategies = strategies;
    }

    public bool IsAllowedConflict(IReadOnlyCollection<IEvent> intendedEvents, IEnumerable<Marten.Events.IEvent> conflictingEvents)
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.IsAllowedConflict(intendedEvents, conflictingEvents))
            {
                return true;
            }
        }

        return false;
    }
}
