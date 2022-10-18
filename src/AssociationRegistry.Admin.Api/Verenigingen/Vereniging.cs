namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Collections.Generic;
using System.Linq;
using AssociationRegistry.Events;
using VCodes;

public class Vereniging
{
    private class State
    {
        public VCode VCode { get; }
        public string Naam { get; }

        private State(string vCode, string naam)
        {
            VCode = new VCode(vCode);
            Naam = naam;
        }

        public static State Apply(VerenigingWerdGeregistreerd @event)
            => new(@event.VCode, @event.Naam);
    }

    private readonly State _state;

    public string VCode
        => _state.VCode;


    public Vereniging(VCode vCode, string naam)
    {
        _state = State.Apply(new VerenigingWerdGeregistreerd(vCode, naam));

        Events = Events.Append(new VerenigingWerdGeregistreerd(vCode, naam));
    }

    public IEnumerable<IEvent> Events { get; } = new List<IEvent>();
}
