namespace AssociationRegistry.Vereniging;

using Emails;
using Events;
using Exceptions;
using Framework;
using Marten.Schema;
using SocialMedias;
using TelefoonNummers;

public class Vereniging : IHasVersion
{
    private VerenigingState _state = new();

    public Vereniging()
    {
    }

    private Vereniging(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Startdatum? startdatum,
        Contactgegevens contactgegevens,
        Locaties locaties,
        Vertegenwoordigers vertegenwoordigers,
        HoofdactiviteitenVerenigingsloket hoofdactiviteitenVerenigingsloketLijst)
    {
        var feitelijkeVerenigingWerdGeregistreerdEvent = new FeitelijkeVerenigingWerdGeregistreerd(
            vCode,
            VerenigingsType.FeitelijkeVereniging.Code,
            naam,
            korteNaam ?? string.Empty,
            korteBeschrijving ?? string.Empty,
            startdatum?.Datum,
            ToEventContactgegevens(contactgegevens.ToArray()),
            ToLocatieLijst(locaties.ToArray()),
            ToVertegenwoordigersLijst(vertegenwoordigers.ToArray()),
            ToHoofdactiviteitenLijst(hoofdactiviteitenVerenigingsloketLijst.ToArray()));

        AddEvent(feitelijkeVerenigingWerdGeregistreerdEvent);
    }

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
            Contactgegevens.FromArray(contactgegevens),
            Locaties.FromArray(locaties),
            Vertegenwoordigers.FromArray(vertegenwoordigers),
            HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloketLijst));
    }

    private static void MustNotBeInFuture(Startdatum startdatum, DateOnly today)
        => Throw<StardatumIsInFuture>.If(startdatum.IsInFuture(today));

    private static FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven[] ToEventContactgegevens(Contactgegeven[] contactgegevens)
        => contactgegevens.Select(FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven.With).ToArray();

    private static FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket[] ToHoofdactiviteitenLijst(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket.With).ToArray();

    private static FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger[] ToVertegenwoordigersLijst(Vertegenwoordiger[] vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger.With).ToArray();

    private static FeitelijkeVerenigingWerdGeregistreerd.Locatie[] ToLocatieLijst(Locatie[] locatieLijst)
        => locatieLijst.Select(FeitelijkeVerenigingWerdGeregistreerd.Locatie.With).ToArray();

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

    public void WijzigHoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        if (HoofdactiviteitenVerenigingsloket.Equals(hoofdactiviteitenVerenigingsloket, _state.HoofdactiviteitenVerenigingsloket))
            return;

        var hoofdactiviteiten = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloket);
        AddEvent(HoofdactiviteitenVerenigingsloketWerdenGewijzigd.With(hoofdactiviteiten.ToArray()));
    }

    public void VoegVertegenwoordigerToe(Vertegenwoordiger vertegenwoordiger)
    {
        _state.Vertegenwoordigers.MustNotHaveDuplicateOf(vertegenwoordiger);
        _state.Vertegenwoordigers.MustNotHaveMultiplePrimary(vertegenwoordiger);

        vertegenwoordiger = vertegenwoordiger with { VertegenwoordigerId = _state.Vertegenwoordigers.NextId };
        AddEvent(VertegenwoordigerWerdToegevoegd.With(vertegenwoordiger));
    }

    public void WijzigVertegenwoordiger(int vertegenwoordigerId, string? rol, string? roepnaam, Email? email, TelefoonNummer? telefoonNummer, TelefoonNummer? mobiel, SocialMedia? socialMedia, bool? isPrimair)
    {
        _state.Vertegenwoordigers.MustContain(vertegenwoordigerId);

        if (_state.Vertegenwoordigers[vertegenwoordigerId].WouldBeEquivalent(rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair, out var updatedVertegenwoordiger))
            return;

        _state.Vertegenwoordigers.MustNotHaveMultiplePrimary(updatedVertegenwoordiger);

        AddEvent(VertegenwoordigerWerdGewijzigd.With(updatedVertegenwoordiger));
    }

    public void VerwijderVertegenwoordiger(int vertegenwoordigerId)
    {
        _state.Vertegenwoordigers.MustContain(vertegenwoordigerId);

        var vertegenwoordiger = _state.Vertegenwoordigers[vertegenwoordigerId];

        AddEvent(VertegenwoordigerWerdVerwijderd.With(vertegenwoordiger));
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
