namespace AssociationRegistry.Vereniging;

using System;
using System.Collections.Generic;
using System.Linq;
using ContactInfo;
using Framework;
using KboNummers;
using Locaties;
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
            VCode = VCode.Create(vCode);
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
        ContactLijst contactLijst,
        LocatieLijst locatieLijst,
        DateOnly today)
    {
        var verenigingWerdGeregistreerdEvent = new VerenigingWerdGeregistreerd(
            vCode,
            naam,
            korteNaam,
            korteBeschrijving,
            startdatum?.Value,
            kboNummer?.ToString(),
            VerenigingWerdGeregistreerd.ContactInfo.FromContactInfoLijst(contactLijst),
            ToLocatieLijst(locatieLijst),
            today);

        _state = State.Apply(verenigingWerdGeregistreerdEvent);
        Events = Events.Append(verenigingWerdGeregistreerdEvent);
    }

    private static VerenigingWerdGeregistreerd.Locatie[] ToLocatieLijst(LocatieLijst locatieLijst)
        => locatieLijst.Select(ToLocatie).ToArray();

    private static VerenigingWerdGeregistreerd.Locatie ToLocatie(Locatie loc)
        => new(
            loc.Naam,
            loc.Straatnaam,
            loc.Huisnummer,
            loc.Busnummer,
            loc.Postcode,
            loc.Gemeente,
            loc.Land,
            loc.IsHoofdlocatie,
            loc.Locatietype);

    public IEnumerable<IEvent> Events { get; } = new List<IEvent>();
}
