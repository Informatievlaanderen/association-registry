namespace AssociationRegistry.Vereniging;

using System;
using System.Collections.Generic;
using System.Linq;
using ContactInfo;
using Events;
using Framework;
using KboNummers;
using Locaties;
using Marten.Schema;
using Startdatums;
using VCodes;
using VerenigingsNamen;

public class Vereniging
{
    public record State
    {
        public VCode VCode { get; set; }
        public string Naam { get; set; }

        public State(string vCode)
        {
            VCode = VCode.Create(vCode);
        }
    }

    private State _state;

    [Identity]
    public string VCode
    {
        get => _state.VCode;
        set => _state = _state with { VCode = VCodes.VCode.Create(value) };
    }

    public Vereniging()
    {
    }

    private Vereniging(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Startdatum? startdatum,
        KboNummer? kboNummer,
        ContactLijst contactLijst,
        LocatieLijst locatieLijst,
        DateOnly datumLaatsteAanpassing)
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
            datumLaatsteAanpassing);

        Apply(verenigingWerdGeregistreerdEvent);
        UncommittedEvents = UncommittedEvents.Append(verenigingWerdGeregistreerdEvent);
    }

    public static Vereniging Registreer(VCode vCode, VerenigingsNaam naam, string? korteNaam, string? korteBeschrijving, Startdatum? startdatum, KboNummer? kboNummer, ContactLijst contactLijst, LocatieLijst locatieLijst, DateOnly datumLaatsteAanpassing)
        => new(vCode, naam, korteNaam, korteBeschrijving, startdatum, kboNummer, contactLijst, locatieLijst, datumLaatsteAanpassing);

    public static Vereniging Registreer(VCode vCode, VerenigingsNaam naam, DateOnly datumLaatsteAanpassing)
        => new(vCode: vCode, naam: naam, null, null, null, null, ContactLijst.Empty, LocatieLijst.Empty, datumLaatsteAanpassing);

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

    public IEnumerable<IEvent> UncommittedEvents { get; private set; } = new List<IEvent>();

    public void ClearEvents()
    {
        UncommittedEvents = new List<IEvent>();
    }

    public void WijzigNaam(VerenigingsNaam naam)
    {
        if (naam.Equals(_state.Naam)) return;

        var @event = new NaamWerdGewijzigd(VCode, naam);
        Apply(@event);
        UncommittedEvents = UncommittedEvents.Append(@event);
    }

    public void Apply(VerenigingWerdGeregistreerd @event)
        => _state = new State(@event.VCode) { Naam = @event.Naam };

    public void Apply(NaamWerdGewijzigd @event)
        => _state = _state with { Naam = @event.Naam };

}
