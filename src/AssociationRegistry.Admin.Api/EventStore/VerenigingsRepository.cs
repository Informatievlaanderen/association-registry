namespace AssociationRegistry.Admin.Api.EventStore;

using System.Linq;
using System.Threading.Tasks;
using Framework;
using Vereniging;

public class VerenigingsRepository : IVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public VerenigingsRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<long> Save(Vereniging vereniging, CommandMetadata metadata)
        => await _eventStore.Save(vereniging.VCode, metadata, vereniging.Events.ToArray());
}
