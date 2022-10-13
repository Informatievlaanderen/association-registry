namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Collections.Generic;
using System.Linq;
using Events;

public class Vereniging
{
    private class State
    {
        public string VCode { get; }
        public string Naam { get; }

        private State(string vCode, string naam)
        {
            VCode = vCode;
            Naam = naam;
        }

        public static State Apply(VerenigingWerdGeregistreerd @event)
            => new(@event.VCode, @event.Naam);
    }

    private readonly State _state;

    public string VCode
        => _state.VCode;


    public Vereniging(string vCode, string naam)
    {
        _state = State.Apply(new VerenigingWerdGeregistreerd(vCode, naam));

        Events = Events.Append(new VerenigingWerdGeregistreerd(vCode, naam));
    }

    public IEnumerable<IEvent> Events { get; } = new List<IEvent>();
}
