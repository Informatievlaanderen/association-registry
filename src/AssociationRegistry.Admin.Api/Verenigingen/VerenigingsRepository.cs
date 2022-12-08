namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Linq;
using System.Threading.Tasks;
using Events;
using Framework;
using Vereniging;

public class VerenigingsRepository : IVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public VerenigingsRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task Save(Vereniging vereniging, CommandMetadata metadata)
    {
        await _eventStore.Save(vereniging.VCode, metadata, vereniging.Events.ToArray());
    }
}
