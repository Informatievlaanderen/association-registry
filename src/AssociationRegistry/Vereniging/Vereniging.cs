namespace AssociationRegistry.Vereniging;

using ContactGegevens;
using ContactGegevens.Exceptions;
using Events;
using Framework;
using Hoofdactiviteiten;
using KboNummers;
using Locaties;
using Marten.Schema;
using RegistreerVereniging;
using Startdatums;
using VCodes;
using VerenigingsNamen;
using Vertegenwoordigers;

public class Vereniging : IHasVersion
{
    private State _state = null!;

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
        Contactgegevens contactgegevens,
        LocatieLijst locatieLijst,
        VertegenwoordigersLijst vertegenwoordigersLijst,
        HoofdactiviteitenVerenigingsloketLijst hoofdactiviteitenVerenigingsloketLijst)
    {
        var verenigingWerdGeregistreerdEvent = new VerenigingWerdGeregistreerd(
            vCode,
            naam,
            korteNaam,
            korteBeschrijving,
            startdatum?.Value,
            kboNummer?.ToString(),
            ToEventContactgegevens(contactgegevens),
            ToLocatieLijst(locatieLijst),
            ToVertegenwoordigersLijst(vertegenwoordigersLijst),
            ToEventData(hoofdactiviteitenVerenigingsloketLijst));

        Apply(verenigingWerdGeregistreerdEvent);
        UncommittedEvents = UncommittedEvents.Append(verenigingWerdGeregistreerdEvent);
    }

    private static VerenigingWerdGeregistreerd.Contactgegeven[] ToEventContactgegevens(Contactgegevens contactgegevens)
    {
        return contactgegevens.Select(c => new VerenigingWerdGeregistreerd.Contactgegeven(c.ContactgegevenId, c.Type, c.Waarde, c.Omschrijving, c.IsPrimair)).ToArray();
    }

    [Identity]
    public string VCode
    {
        get => _state.VCode;
        set => _state = _state with { VCode = VCodes.VCode.Create(value) };
    }

    public IEnumerable<IEvent> UncommittedEvents { get; private set; } = new List<IEvent>();

    public long Version { get; set; }

    public static Vereniging Registreer(VCode vCode, VerenigingsNaam naam, string? korteNaam, string? korteBeschrijving, Startdatum? startdatum, KboNummer? kboNummer, Contactgegeven[] contactgegevenLijst, LocatieLijst locatieLijst, VertegenwoordigersLijst vertegenwoordigersLijst, HoofdactiviteitenVerenigingsloketLijst hoofdactiviteitenVerenigingsloketLijst)
        => new(vCode, naam, korteNaam, korteBeschrijving, startdatum, kboNummer, Contactgegevens.FromArray(contactgegevenLijst), locatieLijst, vertegenwoordigersLijst, hoofdactiviteitenVerenigingsloketLijst);

    public static Vereniging Registreer(VCode vCode, VerenigingsNaam naam)
        => new(vCode, naam, korteNaam: null, korteBeschrijving: null, startdatum: null, kboNummer: null, Contactgegevens.Empty, LocatieLijst.Empty, VertegenwoordigersLijst.Empty, HoofdactiviteitenVerenigingsloketLijst.Empty);

    private static VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] ToEventData(HoofdactiviteitenVerenigingsloketLijst hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(ToEventData).ToArray();

    private static VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket ToEventData(HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new(hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Beschrijving);

    private static VerenigingWerdGeregistreerd.Vertegenwoordiger[] ToVertegenwoordigersLijst(VertegenwoordigersLijst vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(ToVertegenwoordiger).ToArray();

    private static VerenigingWerdGeregistreerd.Vertegenwoordiger ToVertegenwoordiger(Vertegenwoordiger vert)
        => new(vert.Insz, vert.PrimairContactpersoon, vert.Roepnaam, vert.Rol, vert.Voornaam, vert.Achternaam, ToEventContactgegevens(vert.Contactgegevens));

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

    public void ClearEvents()
    {
        UncommittedEvents = new List<IEvent>();
    }

    public void WijzigNaam(VerenigingsNaam naam)
    {
        if (naam.Equals(_state.Naam)) return;

        var @event = new NaamWerdGewijzigd(VCode, naam);
        AddEvent(@event);
    }

    public void WijzigKorteNaam(string korteNaam)
    {
        if (korteNaam.Equals(_state.KorteNaam)) return;

        var @event = new KorteNaamWerdGewijzigd(VCode, korteNaam);
        AddEvent(@event);
    }

    public void WijzigKorteBeschrijving(string korteBeschrijving)
    {
        if (korteBeschrijving.Equals(_state.KorteBeschrijving)) return;

        var @event = new KorteBeschrijvingWerdGewijzigd(VCode, korteBeschrijving);
        AddEvent(@event);
    }

    public void WijzigStartdatum(Startdatum? startdatum)
    {
        if (_state.Startdatum is null && startdatum is null) return;
        if (_state.Startdatum is not null && startdatum is not null && startdatum.Equals(_state.Startdatum)) return;


        var @event = new StartdatumWerdGewijzigd(VCode, startdatum?.Value);
        AddEvent(@event);
    }

    public void VoegContactgegevenToe(Contactgegeven contactgegeven)
    {
        Throw<DuplicateContactgegeven>.If(_state.Contactgegevens.Contains(contactgegeven), Enum.GetName(contactgegeven.Type));
        Throw<MultiplePrimaryContactgegevens>.If(contactgegeven.IsPrimair && _state.Contactgegevens.HasPrimairForType(contactgegeven.Type), Enum.GetName(contactgegeven.Type));
        AddEvent(new ContactgegevenWerdToegevoegd(_state.Contactgegevens.NextId, contactgegeven.Type, contactgegeven.Waarde, contactgegeven.Omschrijving, contactgegeven.IsPrimair));
    }

    public void VerwijderContactgegeven(int contactgegevenId)
    {
        Throw<UnknownContactgegeven>.If(!_state.Contactgegevens.HasKey(contactgegevenId));

        var contactgegeven = _state.Contactgegevens[contactgegevenId];
        AddEvent(
            new ContactgegevenWerdVerwijderd(
                contactgegeven!.ContactgegevenId,
                contactgegeven.Type,
                contactgegeven.Waarde,
                contactgegeven.Omschrijving,
                contactgegeven.IsPrimair));
    }

    public void Apply(VerenigingWerdGeregistreerd @event)
        => _state = new State(@event.VCode)
        {
            Naam = new VerenigingsNaam(@event.Naam),
            KorteNaam = @event.KorteNaam,
            KorteBeschrijving = @event.KorteBeschrijving,
            Startdatum = Startdatum.Create(@event.Startdatum),
            Contactgegevens = @event.Contactgegevens.Aggregate(
                Contactgegevens.Empty,
                (lijst, c) => lijst.Append(
                    new Contactgegeven(
                        c.Type,
                        c.Waarde,
                        c.Omschrijving,
                        c.IsPrimair)
                )
            ),
        };

    public void Apply(NaamWerdGewijzigd @event)
        => _state = _state with { Naam = new VerenigingsNaam(@event.Naam) };

    public void Apply(KorteNaamWerdGewijzigd @event)
        => _state = _state with { KorteNaam = @event.KorteNaam };

    public void Apply(KorteBeschrijvingWerdGewijzigd @event)
        => _state = _state with { KorteBeschrijving = @event.KorteBeschrijving };


    public void Apply(StartdatumWerdGewijzigd @event)
        => _state = _state with { Startdatum = Startdatum.Create(@event.Startdatum) };


    public void Apply(ContactgegevenWerdToegevoegd @event)
    {
        _state = _state with
        {
            Contactgegevens = _state.Contactgegevens.Append(
                new Contactgegeven(
                    @event.Type,
                    @event.Waarde,
                    @event.Omschrijving,
                    @event.IsPrimair)),
        };
    }

    private void AddEvent(IEvent @event)
    {
        try
        {
            Apply((dynamic)@event);
        }
        catch
        {
            // ignored
        }

        UncommittedEvents = UncommittedEvents.Append(@event);
    }

    public record State
    {
        public State(string vCode)
        {
            VCode = VCode.Create(vCode);
        }

        public VCode VCode { get; set; }
        public VerenigingsNaam Naam { get; set; } = null!;
        public string? KorteNaam { get; set; }
        public string? KorteBeschrijving { get; set; }
        public Startdatum? Startdatum { get; set; }
        public Contactgegevens Contactgegevens { get; set; } = Contactgegevens.Empty;
    }
}
