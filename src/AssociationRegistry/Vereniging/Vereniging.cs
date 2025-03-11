namespace AssociationRegistry.Vereniging;

using Emails;
using EventFactories;
using Events;
using Exceptions;
using Framework;
using GemeentenaamDecorator;
using Grar.Clients;
using Grar.Exceptions;
using SocialMedias;
using TelefoonNummers;
using VerenigingWerdVerwijderd = Events.VerenigingWerdVerwijderd;

public class Vereniging : VerenigingsBase, IHydrate<VerenigingState>
{
    public static Vereniging RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Datum? startDatum,
        Doelgroep doelgroep,
        bool uitgeschrevenUitPubliekeDatastroom,
        Contactgegeven[] toeTeVoegenContactgegevens,
        Locatie[] toeTeVoegenLocaties,
        Vertegenwoordiger[] toeTeVoegenVertegenwoordigers,
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst,
        Werkingsgebied[] werkingsgebieden,
        IClock clock)
    {
        Throw<StartdatumMagNietInToekomstZijn>.If(startDatum?.IsInFutureOf(clock.Today) ?? false);

        var toegevoegdeLocaties = Locaties.Empty.VoegToe(toeTeVoegenLocaties);
        var toegevoegdeContactgegevens = Contactgegevens.Empty.VoegToe(toeTeVoegenContactgegevens);
        var toegevoegdeVertegenwoordigers = Vertegenwoordigers.Empty.VoegToe(toeTeVoegenVertegenwoordigers);

        var vereniging = new Vereniging();

        vereniging.AddEvent(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                vCode,
                naam,
                korteNaam ?? string.Empty,
                korteBeschrijving ?? string.Empty,
                startDatum?.Value,
                EventFactory.Doelgroep(doelgroep),
                uitgeschrevenUitPubliekeDatastroom,
                ToEventContactgegevens(toegevoegdeContactgegevens),
                ToLocatieLijst(toegevoegdeLocaties),
                ToVertegenwoordigersLijst(toegevoegdeVertegenwoordigers),
                ToHoofdactiviteitenLijst(HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloketLijst))
            ));

        vereniging.RegistreerWerkingsgebieden(werkingsgebieden);

        return vereniging;
    }

    private void RegistreerWerkingsgebieden(Werkingsgebied[] teRegistrerenWerkingsgebieden)
    {
        var werkingsgebieden = Werkingsgebieden.FromArray(teRegistrerenWerkingsgebieden);

        if (werkingsgebieden.IsNietBepaald)
            return;

        if (werkingsgebieden.IsNietVanToepassing)
        {
            AddEvent(new WerkingsgebiedenWerdenNietVanToepassing(VCode));

            return;
        }

        AddEvent(EventFactory.WerkingsgebiedenWerdenBepaald(VCode, werkingsgebieden.ToArray()));
    }

    private static Registratiedata.Contactgegeven[] ToEventContactgegevens(Contactgegeven[] contactgegevens)
        => contactgegevens.Select(EventFactory.Contactgegeven).ToArray();

    private static Registratiedata.HoofdactiviteitVerenigingsloket[] ToHoofdactiviteitenLijst(
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(EventFactory.HoofdactiviteitVerenigingsloket).ToArray();

    private static Registratiedata.Werkingsgebied[] ToWerkingsgebiedenLijst(
        Werkingsgebied[] werkingsgebieden)
        => werkingsgebieden.Select(EventFactory.Werkingsgebied).ToArray();

    private static Registratiedata.Vertegenwoordiger[] ToVertegenwoordigersLijst(Vertegenwoordiger[] vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(EventFactory.Vertegenwoordiger).ToArray();

    private static Registratiedata.Locatie[] ToLocatieLijst(Locatie[] locatieLijst)
        => locatieLijst.Select(EventFactory.Locatie).ToArray();

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

    public void WijzigStartdatum(Datum? startDatum, IClock clock)
    {
        if (Datum.Equals(State.Startdatum, startDatum))
            return;

        Throw<StartdatumLigtNaEinddatum>.If(State.Einddatum?.IsInPastOf(startDatum) ?? false);
        Throw<StartdatumMagNietInToekomstZijn>.If(startDatum?.IsInFutureOf(clock.Today) ?? false);

        AddEvent(EventFactory.StartdatumWerdGewijzigd(State.VCode, startDatum));
    }

    public void Stop(Datum einddatum, IClock clock)
    {
        if (einddatum == State.Einddatum) return;

        Throw<EinddatumMagNietInToekomstZijn>.If(einddatum.IsInFutureOf(clock.Today));
        Throw<EinddatumLigtVoorStartdatum>.If(einddatum.IsInPastOf(State.Startdatum));

        if (State.IsGestopt)
            AddEvent(EventFactory.EinddatumWerdGewijzigd(einddatum));
        else
            AddEvent(EventFactory.VerenigingWerdGestopt(einddatum));
    }

    public void Verwijder(string reden)
    {
        Throw<VerenigingKanNietVerwijderdWorden>.If(State.IsVerwijderd);
        AddEvent(new VerenigingWerdVerwijderd(VCode, reden));
    }

    public void WijzigDoelgroep(Doelgroep doelgroep)
    {
        if (Doelgroep.Equals(State.Doelgroep, doelgroep)) return;

        AddEvent(EventFactory.DoelgroepWerdGewijzigd(doelgroep));
    }

    public void WijzigHoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        if (HoofdactiviteitenVerenigingsloket.Equals(hoofdactiviteitenVerenigingsloket, State.HoofdactiviteitenVerenigingsloket))
            return;

        Throw<LaatsteHoofdActiviteitKanNietVerwijderdWorden>.If(State.HoofdactiviteitenVerenigingsloket.Any() &&
                                                                !hoofdactiviteitenVerenigingsloket.Any());

        var hoofdactiviteiten = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloket);
        AddEvent(EventFactory.HoofdactiviteitenVerenigingsloketWerdenGewijzigd(hoofdactiviteiten.ToArray()));
    }

    public void WijzigWerkingsgebieden(Werkingsgebied[] werkingsgebieden)
    {
        if (Werkingsgebieden.Equals(werkingsgebieden, State.Werkingsgebieden))
            return;

        if (Werkingsgebieden.Equals(werkingsgebieden, Werkingsgebieden.NietBepaald!))
        {
            AddEvent(new WerkingsgebiedenWerdenNietBepaald(VCode));

            return;
        }

        if (Werkingsgebieden.Equals(werkingsgebieden, Werkingsgebieden.NietVanToepassing!))
        {
            AddEvent(new WerkingsgebiedenWerdenNietVanToepassing(VCode));

            return;
        }

        var werkingsgebiedenData = Werkingsgebieden.FromArray(werkingsgebieden);

        AddEvent(State.Werkingsgebieden == Werkingsgebieden.NietBepaald ||
                 State.Werkingsgebieden == Werkingsgebieden.NietVanToepassing
                     ? EventFactory.WerkingsgebiedenWerdenBepaald(VCode, werkingsgebiedenData.ToArray())
                     : EventFactory.WerkingsgebiedenWerdenGewijzigd(VCode, werkingsgebiedenData.ToArray()));
    }

    public Vertegenwoordiger VoegVertegenwoordigerToe(Vertegenwoordiger vertegenwoordiger)
    {
        var toegevoegdeVertegenwoordiger = State.Vertegenwoordigers.VoegToe(vertegenwoordiger);

        AddEvent(EventFactory.VertegenwoordigerWerdToegevoegd(toegevoegdeVertegenwoordiger));

        return toegevoegdeVertegenwoordiger;
    }

    public void WijzigVertegenwoordiger(
        int vertegenwoordigerId,
        string? rol,
        string? roepnaam,
        Email? email,
        TelefoonNummer? telefoonNummer,
        TelefoonNummer? mobiel,
        SocialMedia? socialMedia,
        bool? isPrimair)
    {
        var gewijzigdeVertegenwoordiger =
            State.Vertegenwoordigers.Wijzig(vertegenwoordigerId, rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair);

        if (gewijzigdeVertegenwoordiger is null)
            return;

        AddEvent(EventFactory.VertegenwoordigerWerdGewijzigd(gewijzigdeVertegenwoordiger));
    }

    public void VerwijderVertegenwoordiger(int vertegenwoordigerId)
    {
        var vertegenwoordiger = State.Vertegenwoordigers.Verwijder(vertegenwoordigerId);
        AddEvent(EventFactory.VertegenwoordigerWerdVerwijderd(vertegenwoordiger));
    }

    public void SchrijfUitUitPubliekeDatastroom()
    {
        if (State.IsUitgeschrevenUitPubliekeDatastroom) return;
        AddEvent(new VerenigingWerdUitgeschrevenUitPubliekeDatastroom());
    }

    public void SchrijfInInPubliekeDatastroom()
    {
        if (!State.IsUitgeschrevenUitPubliekeDatastroom) return;
        AddEvent(new VerenigingWerdIngeschrevenInPubliekeDatastroom());
    }

    public void MarkeerAlsDubbelVan(VCode isDubbelVan)
    {
        Throw<VerenigingKanGeenDubbelWordenVanZichzelf>.If(isDubbelVan.Equals(VCode));

        Throw<AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden>.If(State.IsAuthentiekeVereniging);

        AddEvent(EventFactory.VerenigingWerdGemarkeerdAlsDubbelVan(VCode, isDubbelVan));
    }

    public string CorrigeerMarkeringAlsDubbelVan()
    {
        switch (State.VerenigingStatus)
        {
            case VerenigingStatus.StatusDubbel statusDubbel:
                AddEvent(EventFactory.MarkeringDubbeleVerengingWerdGecorrigeerd(VCode, statusDubbel));

                return statusDubbel.VCodeAuthentiekeVereniging;

            default:
                throw new VerenigingMoetGemarkeerdZijnAlsDubbelOmGecorrigeerdTeKunnenWorden();
        }
    }

    public void VerwerkWeigeringDubbelDoorAuthentiekeVereniging(VCode vCodeAuthentiekeVereniging)
    {
        switch (State.VerenigingStatus)
        {
            case VerenigingStatus.StatusDubbel statusDubbel:
                Throw<ApplicationException>.If(!statusDubbel.VCodeAuthentiekeVereniging.Equals(vCodeAuthentiekeVereniging));
                AddEvent(EventFactory.WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(VCode, statusDubbel));

                break;
        }
    }

    public void VerfijnSubtypeNaarFeitelijkeVereniging()
    {
        if(State.Verenigingssubtype == Verenigingssubtype.FeitelijkeVereniging)
            return;

        AddEvent(EventFactory.SubtypeWerdVerfijndNaarFeitelijkeVereniging(VCode));
    }

    public void ZetSubtypeNaarNietBepaald()
    {
        if(State.Verenigingssubtype == Verenigingssubtype.NietBepaald)
            return;

        AddEvent(EventFactory.SubtypeWerdTerugGezetNaarNietBepaald(VCode));
    }

    public void Hydrate(VerenigingState obj)
    {
        Throw<ActieIsNietToegestaanVoorVerenigingstype>.If( !Verenigingstype.TypeIsVerenigingZonderEigenRechtspersoonlijkheid(obj.Verenigingstype));
        State = obj;
    }

    public async Task NeemAdresDetailOver(
        Registratiedata.Locatie teSynchroniserenLocatie,
        IGrarClient grarClient,
        CancellationToken cancellationToken)
    {
        var adresDetailResponse = await grarClient.GetAddressById(teSynchroniserenLocatie.AdresId!.ToString(), cancellationToken);

        if (!adresDetailResponse.IsActief)
            throw new AdressenregisterReturnedInactiefAdres();

        var postalInformation = await grarClient.GetPostalInformation(adresDetailResponse.Postcode);

        var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
            postalInformation,
            adresDetailResponse.Gemeente);

        var registratieData =
            EventFactory.AdresUitAdressenregister(adresDetailResponse, verrijkteGemeentenaam);

        AddEvent(new AdresWerdOvergenomenUitAdressenregister(VCode, teSynchroniserenLocatie.LocatieId,
                                                             adresDetailResponse.AdresId,
                                                             registratieData));
    }
}
