namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Collections.Generic;
using System.Linq;
using AssociationRegistry.Admin.Api.Events;

public class Vereniging
{
    private class State
    {
        public string VNummer { get; set; }
        public string Naam { get; set; }

        public static State Apply(VerenigingCreated @event)
            => new() { VNummer = @event.VNummer, Naam = @event.Naam };
    }

    private State _state;

    public string VNummer
        => _state.VNummer;


    public Vereniging(string vNummer, string naam)
    {
        _state = State.Apply(new VerenigingCreated(vNummer, naam));

        Events = Events.Append(new VerenigingCreated(vNummer, naam));
    }

    public IEnumerable<IEvent> Events { get; } = new List<IEvent>();
}
