namespace AssociationRegistry.EventStore;

using Framework;
using Vereniging;

public class VerenigingsRepository : IVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public VerenigingsRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<StreamActionResult> Save(Vereniging vereniging, CommandMetadata metadata)
    {
        var events = vereniging.UncommittedEvents.ToArray();
        if (!events.Any())
            return StreamActionResult.Empty;

        return await _eventStore.Save(vereniging.VCode, metadata, events);
    }

    public async Task<Vereniging> Load(VCode vCode, long? expectedVersion)
    {
        var vereniging = await _eventStore.Load<Vereniging>(vCode);

        if (expectedVersion is not null && vereniging.Version != expectedVersion)
            throw new UnexpectedAggregateVersionException();

        return vereniging;
    }
}
