namespace AssociationRegistry.Admin.Api.Verenigingen;

using System;
using System.Collections.Generic;
using System.Linq;
using AssociationRegistry.Vereniging;
using Framework;
using KboNummers;
using Startdatums;
using VCodes;
using VerenigingsNamen;

public class Vereniging
{
    private class State
    {
        public VCode VCode { get; }

        private State(string vCode)
        {
            VCode = new VCode(vCode);
        }

        public static State Apply(VerenigingWerdGeregistreerd @event)
            => new(@event.VCode);
    }

    private readonly State _state;

    public string VCode
        => _state.VCode;

    public Vereniging(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Startdatum? startdatum,
        KboNummer? kboNummer,
        DateOnly today)
    {
        var verenigingWerdGeregistreerdEvent = new VerenigingWerdGeregistreerd(
            vCode,
            naam,
            korteNaam,
            korteBeschrijving,
            startdatum?.Value,
            kboNummer?.ToString(),
            "Actief",
            today);

        _state = State.Apply(verenigingWerdGeregistreerdEvent);
        Events = Events.Append(verenigingWerdGeregistreerdEvent);
    }

    public IEnumerable<IEvent> Events { get; } = new List<IEvent>();
}
