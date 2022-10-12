namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Collections.Generic;
using System.Linq;
using AssociationRegistry.Admin.Api.Events;

public class Vereniging
{
    private class State
    {
        public string VCode { get; set; }
        public string Naam { get; set; }

        public static State Apply(VerenigingWerdGeregistreerd @event)
            => new() { VCode = @event.VCode, Naam = @event.Naam };
    }

    private State _state;

    public string VCode
        => _state.VCode;


    public Vereniging(string vCode, string naam)
    {
        _state = State.Apply(new VerenigingWerdGeregistreerd(vCode, naam));

        Events = Events.Append(new VerenigingWerdGeregistreerd(vCode, naam));
    }

    public IEnumerable<IEvent> Events { get; } = new List<IEvent>();
}
