namespace AssociationRegistry.Admin.Api.Verenigingen;

using System;
using System.Collections.Generic;
using System.Linq;
using AssociationRegistry.Events;
using KboNummers;
using VCodes;
using VerenigingsNamen;

public class Vereniging
{
    private class State
    {
        public VCode VCode { get; }
        public VerenigingsNaam Naam { get; }
        public DateOnly? Startdatum { get; }

        private State(string vCode, string naam, DateOnly? startdatum)
        {
            Startdatum = startdatum;
            VCode = new VCode(vCode);
            Naam = new VerenigingsNaam(naam);
        }

        public static State Apply(VerenigingWerdGeregistreerd @event)
            => new(@event.VCode, @event.Naam, @event.Startdatum.HasValue ? DateOnly.FromDateTime(@event.Startdatum.Value) : null);
    }

    private readonly State _state;

    public string VCode
        => _state.VCode;

    public Vereniging(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        DateOnly? startdatum,
        KboNummer? kboNummer,
        DateOnly laatstGewijzigd)
    {
        var verenigingWerdGeregistreerdEvent = new VerenigingWerdGeregistreerd(
            vCode,
            naam,
            korteNaam,
            korteBeschrijving,
            startdatum?.ToDateTime(TimeOnly.MinValue),
            kboNummer?.ToString(),
            "Actief",
            laatstGewijzigd.ToDateTime(TimeOnly.MinValue));

        _state = State.Apply(verenigingWerdGeregistreerdEvent);
        Events = Events.Append(verenigingWerdGeregistreerdEvent);
    }

    public IEnumerable<IEvent> Events { get; } = new List<IEvent>();
}
