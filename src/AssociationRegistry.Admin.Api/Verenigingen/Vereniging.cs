namespace AssociationRegistry.Admin.Api.Verenigingen;

using System;
using System.Collections.Generic;
using System.Linq;
using AssociationRegistry.Events;
using KboNummers;
using Startdatums;
using VCodes;
using VerenigingsNamen;

public class Vereniging
{
    private class State
    {
        public VCode VCode { get; }
        public VerenigingsNaam Naam { get; }
        public Startdatum? Startdatum { get; }

        private State(string vCode, string naam, Startdatum? startdatum)
        {
            Startdatum = startdatum;
            VCode = new VCode(vCode);
            Naam = new VerenigingsNaam(naam);
        }

        public static State Apply(VerenigingWerdGeregistreerd @event)
            => new(@event.VCode, @event.Naam, Startdatum.Create(@event.Startdatum));
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
