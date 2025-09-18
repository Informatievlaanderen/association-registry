namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using Events;
using Marten;
using OpenTelemetry.Metrics;

public class DuplicateVerenigingsRepository : IDuplicateVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public DuplicateVerenigingsRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<StreamActionResult> Save(
        string aggregateId, IDocumentSession session, CommandMetadata metadata, CancellationToken cancellationToken, IEvent[] events)
    {
        var streamState = await session.Events.FetchStreamStateAsync("DD0001", cancellationToken);

        if (streamState == null)
            return await _eventStore.SaveNew(aggregateId, session, metadata, cancellationToken, events);

        return await _eventStore.Save(aggregateId, streamState.Version, metadata, cancellationToken, events);
    }
}
