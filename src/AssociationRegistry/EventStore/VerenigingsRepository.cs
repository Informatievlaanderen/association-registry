namespace AssociationRegistry.EventStore;

using System.Linq;
using System.Threading.Tasks;
using Framework;
using VCodes;
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
        => await _eventStore.Load<Vereniging>(vCode, expectedVersion);
}
