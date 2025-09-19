namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Marten;

public class DubbelDetectieVerenigingsRepository : IDubbelDetectieVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public DubbelDetectieVerenigingsRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<StreamActionResult> Save(
        string aggregateId, IDocumentSession session, CommandMetadata metadata, CancellationToken cancellationToken, IEvent[] events)
    {
        var streamState = await session.Events.FetchStreamStateAsync(aggregateId, cancellationToken);

        if (streamState == null)
            throw new OngeldigBevestigingsToken();

        return await _eventStore.Save(aggregateId, streamState.Version, metadata, cancellationToken, events);
    }

    public async Task<StreamActionResult> SaveNew(
        string aggregateId, IDocumentSession session, CommandMetadata metadata, CancellationToken cancellationToken, IEvent[] events)
        => await _eventStore.SaveNew(aggregateId, session, metadata, cancellationToken, events);
}

public class OngeldigBevestigingsToken : Exception
{
}
