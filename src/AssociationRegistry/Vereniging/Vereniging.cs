namespace AssociationRegistry.Vereniging;

using Emails;
using Events;
using Exceptions;
using Framework;
using SocialMedias;
using TelefoonNummers;

public class Vereniging : VerenigingsBase, IHydrate<VerenigingState>
{
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
        var vereniging = new Vereniging();
        vereniging.AddEvent(
            new FeitelijkeVerenigingWerdGeregistreerd(
                vCode,
                naam,
                korteNaam ?? string.Empty,
                korteBeschrijving ?? string.Empty,
                startdatum.Datum,
                ToEventContactgegevens(Contactgegevens.FromArray(contactgegevens).ToArray()),
                ToLocatieLijst(Locaties.FromArray(locaties).ToArray()),
                ToVertegenwoordigersLijst(Vertegenwoordigers.FromArray(vertegenwoordigers).ToArray()),
                ToHoofdactiviteitenLijst(HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloketLijst).ToArray())));
        return vereniging;
    }

    public static Vereniging RegistreerAfdeling(
        VCode vCode,
        VerenigingsNaam naam,
        KboNummer kboNummerMoedervereniging,
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
        var vereniging = new Vereniging();
        vereniging.AddEvent(
            new AfdelingWerdGeregistreerd(
                vCode,
                naam,
                new AfdelingWerdGeregistreerd.MoederverenigingsData(kboNummerMoedervereniging, string.Empty, $"Moeder {kboNummerMoedervereniging}"),
                korteNaam ?? string.Empty,
                korteBeschrijving ?? string.Empty,
                startdatum.Datum,
                ToEventContactgegevens(Contactgegevens.FromArray(contactgegevens).ToArray()),
                ToLocatieLijst(Locaties.FromArray(locaties).ToArray()),
                ToVertegenwoordigersLijst(Vertegenwoordigers.FromArray(vertegenwoordigers).ToArray()),
                ToHoofdactiviteitenLijst(HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloketLijst).ToArray())));
        return vereniging;
    }


    private static void MustNotBeInFuture(Startdatum startdatum, DateOnly today)
        => Throw<StardatumIsInFuture>.If(startdatum.IsInFuture(today));

    private static Registratiedata.Contactgegeven[] ToEventContactgegevens(Contactgegeven[] contactgegevens)
        => contactgegevens.Select(Registratiedata.Contactgegeven.With).ToArray();

    private static Registratiedata.HoofdactiviteitVerenigingsloket[] ToHoofdactiviteitenLijst(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(Registratiedata.HoofdactiviteitVerenigingsloket.With).ToArray();

    private static Registratiedata.Vertegenwoordiger[] ToVertegenwoordigersLijst(Vertegenwoordiger[] vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(Registratiedata.Vertegenwoordiger.With).ToArray();

    private static Registratiedata.Locatie[] ToLocatieLijst(Locatie[] locatieLijst)
        => locatieLijst.Select(Registratiedata.Locatie.With).ToArray();

    public void WijzigNaam(VerenigingsNaam naam)
    {
        if (naam.Equals(State.Naam))
            return;

        AddEvent(new NaamWerdGewijzigd(VCode, naam));
    }

    public void WijzigKorteNaam(string korteNaam)
    {
        if (korteNaam.Equals(State.KorteNaam))
            return;

        AddEvent(new KorteNaamWerdGewijzigd(VCode, korteNaam));
    }

    public void WijzigKorteBeschrijving(string korteBeschrijving)
    {
        if (korteBeschrijving.Equals(State.KorteBeschrijving))
            return;

        AddEvent(new KorteBeschrijvingWerdGewijzigd(VCode, korteBeschrijving));
    }

    public void WijzigStartdatum(Startdatum startdatum, IClock clock)
    {
        if (Startdatum.Equals(State.Startdatum, startdatum))
            return;

        MustNotBeInFuture(startdatum, clock.Today);

        AddEvent(new StartdatumWerdGewijzigd(VCode, startdatum.Datum));
    }

    public void VoegContactgegevenToe(Contactgegeven contactgegeven)
    {
        State.Contactgegevens.MustNotHaveDuplicates(contactgegeven);
        State.Contactgegevens.MustNotHaveMultiplePrimaryOfTheSameType(contactgegeven);

        contactgegeven = contactgegeven with { ContactgegevenId = State.Contactgegevens.NextId };

        AddEvent(ContactgegevenWerdToegevoegd.With(contactgegeven));
    }

    public void WijzigContactgegeven(int contactgegevenId, string? waarde, string? beschrijving, bool? isPrimair)
    {
        State.Contactgegevens.MustContain(contactgegevenId);

        if (State.Contactgegevens[contactgegevenId].WouldBeEquivalent(waarde, beschrijving, isPrimair, out var updatedContactgegeven))
            return;

        State.Contactgegevens.MustNotHaveDuplicates(updatedContactgegeven);
        State.Contactgegevens.MustNotHaveMultiplePrimaryOfTheSameType(updatedContactgegeven);

        AddEvent(ContactgegevenWerdGewijzigd.With(updatedContactgegeven));
    }

    public void VerwijderContactgegeven(int contactgegevenId)
    {
        State.Contactgegevens.MustContain(contactgegevenId);

        var contactgegeven = State.Contactgegevens[contactgegevenId];

        AddEvent(ContactgegevenWerdVerwijderd.With(contactgegeven));
    }

    public void WijzigHoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        if (HoofdactiviteitenVerenigingsloket.Equals(hoofdactiviteitenVerenigingsloket, State.HoofdactiviteitenVerenigingsloket))
            return;

        var hoofdactiviteiten = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloket);
        AddEvent(HoofdactiviteitenVerenigingsloketWerdenGewijzigd.With(hoofdactiviteiten.ToArray()));
    }

    public void VoegVertegenwoordigerToe(Vertegenwoordiger vertegenwoordiger)
    {
        State.Vertegenwoordigers.MustNotHaveDuplicateOf(vertegenwoordiger);
        State.Vertegenwoordigers.MustNotHaveMultiplePrimary(vertegenwoordiger);

        vertegenwoordiger = vertegenwoordiger with { VertegenwoordigerId = State.Vertegenwoordigers.NextId };
        AddEvent(VertegenwoordigerWerdToegevoegd.With(vertegenwoordiger));
    }

    public void WijzigVertegenwoordiger(int vertegenwoordigerId, string? rol, string? roepnaam, Email? email, TelefoonNummer? telefoonNummer, TelefoonNummer? mobiel, SocialMedia? socialMedia, bool? isPrimair)
    {
        State.Vertegenwoordigers.MustContain(vertegenwoordigerId);

        if (State.Vertegenwoordigers[vertegenwoordigerId].WouldBeEquivalent(rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair, out var updatedVertegenwoordiger))
            return;

        State.Vertegenwoordigers.MustNotHaveMultiplePrimary(updatedVertegenwoordiger);

        AddEvent(VertegenwoordigerWerdGewijzigd.With(updatedVertegenwoordiger));
    }

    public void VerwijderVertegenwoordiger(int vertegenwoordigerId)
    {
        State.Vertegenwoordigers.MustContain(vertegenwoordigerId);

        var vertegenwoordiger = State.Vertegenwoordigers[vertegenwoordigerId];

        AddEvent(VertegenwoordigerWerdVerwijderd.With(vertegenwoordiger));
    }

    public void Hydrate(VerenigingState obj)
    {
        Throw<UnsupportedOperationForVerenigingstype>.If(obj.Verenigingstype != Verenigingstype.FeitelijkeVereniging && obj.Verenigingstype != Verenigingstype.Afdeling);
        State = obj;
    }
}
