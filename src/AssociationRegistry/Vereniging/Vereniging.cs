namespace AssociationRegistry.Vereniging;

using Events;
using Exceptions;
using Framework;
using Marten.Schema;

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
        Contactgegeven[] contactgegevens,
        Locatie[] locatieLijst,
        Vertegenwoordiger[] vertegenwoordigersLijst,
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
    {
        var verenigingWerdGeregistreerdEvent = new VerenigingWerdGeregistreerd(
            vCode,
            naam,
            korteNaam,
            korteBeschrijving,
            startdatum?.Datum,
            kboNummer?.ToString(),
            ToEventContactgegevens(Contactgegevens.FromArray(contactgegevens).ToArray()),
            ToLocatieLijst(locatieLijst),
            ToVertegenwoordigersLijst(vertegenwoordigersLijst),
            ToEventData(hoofdactiviteitenVerenigingsloketLijst));

        Apply(verenigingWerdGeregistreerdEvent);
        UncommittedEvents = UncommittedEvents.Append(verenigingWerdGeregistreerdEvent);
    }

    private static VerenigingWerdGeregistreerd.Contactgegeven[] ToEventContactgegevens(Contactgegeven[] contactgegevens)
        => contactgegevens.Select(VerenigingWerdGeregistreerd.Contactgegeven.With).ToArray();

    [Identity]
    public string VCode
    {
        get => _state.VCode;
        set => _state = _state with { VCode = AssociationRegistry.Vereniging.VCode.Create(value) };
    }

    public IEnumerable<IEvent> UncommittedEvents { get; private set; } = new List<IEvent>();

    public long Version { get; set; }

    public static Vereniging Registreer(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Startdatum startdatum,
        KboNummer? kboNummer,
        Contactgegeven[] contactgegevens,
        Locatie[] locatieLijst,
        Vertegenwoordiger[] vertegenwoordigersLijst,
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst,
        IClock clock)
    {
        MustNotBeInFuture(startdatum, clock.Today);

        return new Vereniging(
            vCode,
            naam,
            korteNaam,
            korteBeschrijving,
            startdatum,
            kboNummer,
            contactgegevens,
            locatieLijst,
            vertegenwoordigersLijst,
            hoofdactiviteitenVerenigingsloketLijst);
    }

    private static void MustNotBeInFuture(Startdatum startdatum, DateOnly today)
        => Throw<InvalidStartdatumFuture>.If(startdatum.IsInFuture(today));

    public static Vereniging Registreer(VCode vCode, VerenigingsNaam naam)
        => new(
            vCode,
            naam,
            korteNaam: null,
            korteBeschrijving: null,
            startdatum: null,
            kboNummer: null,
            Array.Empty<Contactgegeven>(),
            Array.Empty<Locatie>(),
            Array.Empty<Vertegenwoordiger>(),
            Array.Empty<HoofdactiviteitVerenigingsloket>());

    private static VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] ToEventData(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(ToEventData).ToArray();

    private static VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket ToEventData(HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new(hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Beschrijving);

    private static VerenigingWerdGeregistreerd.Vertegenwoordiger[] ToVertegenwoordigersLijst(Vertegenwoordiger[] vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(ToVertegenwoordiger).ToArray();

    private static VerenigingWerdGeregistreerd.Vertegenwoordiger ToVertegenwoordiger(Vertegenwoordiger vert)
        => new(vert.Insz, vert.PrimairContactpersoon, vert.Roepnaam, vert.Rol, vert.Voornaam, vert.Achternaam, ToEventContactgegevens(vert.Contactgegevens));

    private static VerenigingWerdGeregistreerd.Locatie[] ToLocatieLijst(Locatie[] locatieLijst)
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
            loc.Hoofdlocatie,
            loc.Locatietype);

    public void ClearEvents()
    {
        UncommittedEvents = new List<IEvent>();
    }

    public void WijzigNaam(VerenigingsNaam naam)
    {
        if (naam.Equals(_state.Naam))
            return;

        AddEvent(new NaamWerdGewijzigd(VCode, naam));
    }

    public void WijzigKorteNaam(string korteNaam)
    {
        if (korteNaam.Equals(_state.KorteNaam))
            return;

        AddEvent(new KorteNaamWerdGewijzigd(VCode, korteNaam));
    }

    public void WijzigKorteBeschrijving(string korteBeschrijving)
    {
        if (korteBeschrijving.Equals(_state.KorteBeschrijving))
            return;

        AddEvent(new KorteBeschrijvingWerdGewijzigd(VCode, korteBeschrijving));
    }

    public void WijzigStartdatum(Startdatum? startdatum)
    {
        if (Startdatum.Equals(_state.Startdatum, startdatum))
            return;

        AddEvent(new StartdatumWerdGewijzigd(VCode, startdatum?.Datum));
    }

    public void VoegContactgegevenToe(Contactgegeven contactgegeven)
    {
        _state.Contactgegevens.MustNotHaveDuplicates(contactgegeven);
        _state.Contactgegevens.MustNotHaveMultiplePrimaryOfTheSameType(contactgegeven);

        contactgegeven = contactgegeven with { ContactgegevenId = _state.Contactgegevens.NextId };

        AddEvent(ContactgegevenWerdToegevoegd.With(contactgegeven));
    }

    public void WijzigContactgegeven(int contactgegevenId, string? waarde, string? omschrijving, bool? isPrimair)
    {
        _state.Contactgegevens.MustContain(contactgegevenId);

        if (_state.Contactgegevens[contactgegevenId].WouldBeEquivalent(waarde, omschrijving, isPrimair, out var updatedContactgegeven))
            return;

        _state.Contactgegevens.MustNotHaveDuplicates(updatedContactgegeven);
        _state.Contactgegevens.MustNotHaveMultiplePrimaryOfTheSameType(updatedContactgegeven);

        AddEvent(ContactgegevenWerdGewijzigd.With(updatedContactgegeven));
    }

    public void VerwijderContactgegeven(int contactgegevenId)
    {
        _state.Contactgegevens.MustContain(contactgegevenId);

        var contactgegeven = _state.Contactgegevens[contactgegevenId];

        AddEvent(ContactgegevenWerdVerwijderd.With(contactgegeven));
    }

    public void Apply(VerenigingWerdGeregistreerd @event)
        => _state = new State(@event.VCode)
        {
            Naam = new VerenigingsNaam(@event.Naam),
            KorteNaam = @event.KorteNaam,
            KorteBeschrijving = @event.KorteBeschrijving,
            Startdatum = new Startdatum(@event.Startdatum),
            Contactgegevens = @event.Contactgegevens.Aggregate(
                Contactgegevens.Empty,
                (lijst, c) => lijst.Append(
                    Contactgegeven.FromEvent(
                        c.ContactgegevenId,
                        ContactgegevenType.Parse(c.Type),
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
        => _state = _state with { Startdatum = Startdatum.Hydrate(@event.Startdatum) };


    public void Apply(ContactgegevenWerdToegevoegd @event)
    {
        _state = _state with
        {
            Contactgegevens = _state.Contactgegevens.Append(
                Contactgegeven.FromEvent(
                    @event.ContactgegevenId,
                    ContactgegevenType.Parse(@event.Type),
                    @event.Waarde,
                    @event.Omschrijving,
                    @event.IsPrimair)),
        };
    }

    public void Apply(ContactgegevenWerdVerwijderd @event)
    {
        _state = _state with
        {
            Contactgegevens = _state.Contactgegevens.Remove(@event.ContactgegevenId),
        };
    }

    public void Apply(ContactgegevenWerdGewijzigd @event)
    {
        _state = _state with
        {
            Contactgegevens = _state.Contactgegevens.Replace(
                Contactgegeven.FromEvent(
                    @event.ContactgegevenId,
                    ContactgegevenType.Parse(@event.Type),
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
