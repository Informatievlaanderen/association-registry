namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using DecentraalBeheer.Vereniging;
using Framework;
using Marten;

public class NewAggregateSession : INewAggregateSession
{
    private readonly IEventStore _eventStore;

    public NewAggregateSession(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<StreamActionResult> SaveNew(
        Vereniging vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    )
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.SaveNew(
            aggregateId: vereniging.VCode,
            metadata: metadata,
            cancellationToken: cancellationToken,
            events: events
        );
    }

    public async Task<StreamActionResult> SaveNew(
        VerenigingMetRechtspersoonlijkheid vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    )
    {
        var events = vereniging.UncommittedEvents.ToArray();

        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.SaveNewKbo(
            aggregateId: vereniging.VCode,
            metadata: metadata,
            cancellationToken: cancellationToken,
            events: events
        );
    }
}
