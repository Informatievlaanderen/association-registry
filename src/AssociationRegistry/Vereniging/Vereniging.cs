namespace AssociationRegistry.Vereniging;

using Events;
using Exceptions;
using Framework;
using Marten.Schema;

public class Vereniging : IHasVersion
{
    private VerenigingState _state = new();

    [Identity]
    public string VCode
    {
        get => _state.VCode;
        set => _state = _state with { VCode = AssociationRegistry.Vereniging.VCode.Create(value) };
    }

    public IEnumerable<IEvent> UncommittedEvents { get; private set; } = new List<IEvent>();

    public long Version { get; set; }

    public Vereniging()
    {
    }

    private Vereniging(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Startdatum? startdatum,
        KboNummer kboNummer,
        Contactgegevens contactgegevens,
        Locaties locaties,
        Vertegenwoordigers vertegenwoordigers,
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
    {
        var verenigingWerdGeregistreerdEvent = new VerenigingWerdGeregistreerd(
            vCode,
            naam,
            korteNaam ?? string.Empty,
            korteBeschrijving ?? string.Empty,
            startdatum?.Datum,
            kboNummer.ToString(),
            ToEventContactgegevens(contactgegevens.ToArray()),
            ToLocatieLijst(locaties.ToArray()),
            ToVertegenwoordigersLijst(vertegenwoordigers.ToArray()),
            ToHoofdactiviteitenLijst(hoofdactiviteitenVerenigingsloketLijst));

        AddEvent(verenigingWerdGeregistreerdEvent);
    }

    public static Vereniging Registreer(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Startdatum startdatum,
        KboNummer kboNummer,
        Contactgegeven[] contactgegevens,
        Locatie[] locaties,
        Vertegenwoordiger[] vertegenwoordigers,
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
            Contactgegevens.FromArray(contactgegevens),
            Locaties.FromArray(locaties),
            Vertegenwoordigers.FromArray(vertegenwoordigers),
            hoofdactiviteitenVerenigingsloketLijst);
    }

    private static void MustNotBeInFuture(Startdatum startdatum, DateOnly today)
        => Throw<StardatumIsInFuture>.If(startdatum.IsInFuture(today));

    private static VerenigingWerdGeregistreerd.Contactgegeven[] ToEventContactgegevens(Contactgegeven[] contactgegevens)
        => contactgegevens.Select(VerenigingWerdGeregistreerd.Contactgegeven.With).ToArray();

    private static VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] ToHoofdactiviteitenLijst(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket.With).ToArray();

    private static VerenigingWerdGeregistreerd.Vertegenwoordiger[] ToVertegenwoordigersLijst(Vertegenwoordiger[] vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(VerenigingWerdGeregistreerd.Vertegenwoordiger.With).ToArray();

    private static VerenigingWerdGeregistreerd.Locatie[] ToLocatieLijst(Locatie[] locatieLijst)
        => locatieLijst.Select(VerenigingWerdGeregistreerd.Locatie.With).ToArray();

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

    public void WijzigStartdatum(Startdatum startdatum, IClock clock)
    {
        if (Startdatum.Equals(_state.Startdatum, startdatum))
            return;

        MustNotBeInFuture(startdatum, clock.Today);

        AddEvent(new StartdatumWerdGewijzigd(VCode, startdatum.Datum));
    }

    public void VoegContactgegevenToe(Contactgegeven contactgegeven)
    {
        _state.Contactgegevens.MustNotHaveDuplicates(contactgegeven);
        _state.Contactgegevens.MustNotHaveMultiplePrimaryOfTheSameType(contactgegeven);

        contactgegeven = contactgegeven with { ContactgegevenId = _state.Contactgegevens.NextId };

        AddEvent(ContactgegevenWerdToegevoegd.With(contactgegeven));
    }

    public void WijzigContactgegeven(int contactgegevenId, string? waarde, string? beschrijving, bool? isPrimair)
    {
        _state.Contactgegevens.MustContain(contactgegevenId);

        if (_state.Contactgegevens[contactgegevenId].WouldBeEquivalent(waarde, beschrijving, isPrimair, out var updatedContactgegeven))
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

    private void AddEvent(IEvent @event)
    {
        Apply(@event);
        UncommittedEvents = UncommittedEvents.Append(@event);
    }

    public void Apply(dynamic @event)
    {
        _state = _state.Apply(@event);
    }
}
