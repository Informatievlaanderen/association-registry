﻿namespace AssociationRegistry.EventStore;

using System.Linq;
using System.Threading.Tasks;
using Framework;
using Marten;
using VCodes;
using Vereniging;

public class VerenigingsRepository : IVerenigingsRepository
{
    private readonly IEventStore _eventStore;

    public VerenigingsRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<long> Save(Vereniging vereniging, CommandMetadata metadata)
        => await _eventStore.Save(vereniging.VCode, metadata, vereniging.UncommittedEvents.ToArray());

    public async Task<Vereniging> Load(VCode vCode)
        => await _eventStore.Load<Vereniging>(vCode);

    // public async Task<Vereniging> Load(VCode vCode)
    // {
    //     var events = await _eventStore.Load(vCode.Value);
    //     return new Vereniging();
    // }
}
