namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Subtypes.Subvereniging;
using Adressen;
using Bankrekeningen;
using Events;
using Framework;
using Emails;
using Events.Factories;
using Exceptions;
using Geotags;
using ImTools;
using Magda.Persoon;
using Microsoft.Extensions.Logging;
using SocialMedias;
using TelefoonNummers;
using VerenigingWerdVerwijderd = Events.VerenigingWerdVerwijderd;

public class Vereniging : VerenigingsBase, IHydrate<VerenigingState>
{
    public static async Task<Vereniging> RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
        RegistratieDataVerenigingZonderEigenRechtspersoonlijkheid registratieData,
        bool potentialDuplicatesSkipped,
        string bevestigingsToken,
        IVCodeService vCodeService,
        IClock clock)
    {
        Throw<StartdatumMagNietInToekomstZijn>.If(registratieData.Startdatum?.IsInFutureOf(clock.Today) ?? false);

        var vCode = await vCodeService.GetNext();

        var toegevoegdeLocaties = Locaties.Empty.VoegToe(registratieData.Locaties);
        var toegevoegdeContactgegevens = Contactgegevens.Empty.VoegToe(registratieData.Contactgegevens);
        var toegevoegdeVertegenwoordigers = Vertegenwoordigers.Empty.VoegToe(registratieData.Vertegenwoordigers);

        var vereniging = new Vereniging();

        vereniging.AddEvent(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                vCode,
                registratieData.Naam,
                registratieData.KorteNaam ?? string.Empty,
                registratieData.KorteBeschrijving ?? string.Empty,
                registratieData.Startdatum?.Value,
                EventFactory.Doelgroep(registratieData.Doelgroep),
                registratieData.IsUitgeschrevenUitPubliekeDatastroom,
                ToEventContactgegevens(toegevoegdeContactgegevens),
                ToLocatieLijst(toegevoegdeLocaties),
                ToVertegenwoordigersLijst(toegevoegdeVertegenwoordigers),
                ToHoofdactiviteitenLijst(HoofdactiviteitenVerenigingsloket.FromArray(registratieData.HoofdactiviteitenVerenigingsloket)),
                potentialDuplicatesSkipped
                    ? Registratiedata.DuplicatieInfo.BevestigdGeenDuplicaat(bevestigingsToken)
                    : Registratiedata.DuplicatieInfo.GeenDuplicaten
            ));

        vereniging.RegistreerWerkingsgebieden(registratieData.Werkingsgebieden);

        return vereniging;
    }

    public void NeemAdresDetailsOver(Locatie[] metAdresId, IReadOnlyDictionary<string, Adres> verrijkteAdressenUitGrar)
    {
        foreach (var locatieMetAdresId in metAdresId)
        {
            var adres = verrijkteAdressenUitGrar[locatieMetAdresId.AdresId!.Bronwaarde];

            AddEvent(new AdresWerdOvergenomenUitAdressenregister(VCode, locatieMetAdresId.LocatieId,
                                                                       new Registratiedata.AdresId(
                                                                           locatieMetAdresId.AdresId!.Adresbron.Code,
                                                                           locatieMetAdresId.AdresId.Bronwaarde),
                                                                       new Registratiedata.AdresUitAdressenregister(
                                                                           adres.Straatnaam, adres.Huisnummer,
                                                                           adres.Busnummer, adres.Postcode,
                                                                           adres.Gemeente.Naam)));
        }
    }

    public async Task BerekenGeotags(IGeotagsService geotagsService)
    {
        var geotags = await geotagsService.CalculateGeotags(State.Locaties, State.Werkingsgebieden);

        if (State.Geotags.Equals(geotags))
            return;

        AddEvent(EventFactory.GeotagsWerdenBepaald(VCode, geotags));
    }

    public void RegistreerWerkingsgebieden(Werkingsgebied[] teRegistrerenWerkingsgebieden)
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

    public bool WijzigWerkingsgebieden(Werkingsgebied[] werkingsgebieden)
    {
        if (Werkingsgebieden.Equals(werkingsgebieden, State.Werkingsgebieden))
            return false;

        if (Werkingsgebieden.Equals(werkingsgebieden, Werkingsgebieden.NietBepaald!))
        {
            AddEvent(new WerkingsgebiedenWerdenNietBepaald(VCode));

            return true;
        }

        if (Werkingsgebieden.Equals(werkingsgebieden, Werkingsgebieden.NietVanToepassing!))
        {
            AddEvent(new WerkingsgebiedenWerdenNietVanToepassing(VCode));

            return true;
        }

        var werkingsgebiedenData = Werkingsgebieden.FromArray(werkingsgebieden);

        AddEvent(State.Werkingsgebieden == Werkingsgebieden.NietBepaald ||
                 State.Werkingsgebieden == Werkingsgebieden.NietVanToepassing
                     ? EventFactory.WerkingsgebiedenWerdenBepaald(VCode, werkingsgebiedenData.ToArray())
                     : EventFactory.WerkingsgebiedenWerdenGewijzigd(VCode, werkingsgebiedenData.ToArray()));

        return true;
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
        State.Verenigingssubtype
             .VerFijnNaarFeitelijkeVereniging(VCode)
             .ForEach(AddEvent);
    }

    public void ZetSubtypeNaarNietBepaald()
    {
        State.Verenigingssubtype
             .ZetSubtypeNaarNietBepaald(VCode)
             .ForEach(AddEvent);
    }

    public void VerfijnNaarSubvereniging(SubverenigingVanDto subverenigingVan)
    {
        Throw<VerenigingKanGeenSubverenigingWordenWaarvanZijAlReedsLidIs>.If(
            AndereVerenigingIsReedsEenLid(subverenigingVan.AndereVereniging));

        State.Verenigingssubtype
             .VerFijnNaarSubvereniging(VCode, subverenigingVan)
             .ForEach(AddEvent);
    }

    public void Hydrate(VerenigingState obj)
    {
        Throw<ActieIsNietToegestaanVoorVerenigingstype>.If(
            !Verenigingstype.TypeIsVerenigingZonderEigenRechtspersoonlijkheid(obj.Verenigingstype));

        State = obj;
    }

    private bool AndereVerenigingIsReedsEenLid(VCode? andereVereniging)
        => State.Lidmaatschappen.Any(x => x.AndereVereniging == andereVereniging);

    public (Locatie[] metAdresId, Locatie[] zonderAdresId) GeefLocatiesMetEnZonderAdresId()
        => State.Locaties.Partition(x => x.AdresId is not null);

    public async Task SchrijfVertegenwoordigersIn(IMagdaGeefPersoonService magdaGeefPersoonService, CommandMetadata envelopeMetadata, CancellationToken cancellationToken, ILogger logger)
    {
        var vertegenwoordigers = State.Vertegenwoordigers.Where(x => !x.BevestigdDoorKsz)
                                      .ToArray();

        if(!vertegenwoordigers.Any())
            return;

        logger.LogInformation($"SchrijfVertegenwoordigersIn started for VCode {VCode} with {vertegenwoordigers.Length} vertegenwoordigers");


        foreach (var vertegenwoordiger in vertegenwoordigers)
        {
            try
            {
                var persoonUitKsz = await magdaGeefPersoonService.GeefPersoon(GeefPersoonRequest.From(vertegenwoordiger), envelopeMetadata, cancellationToken);
                if(persoonUitKsz.Overleden)
                    AddEvent(new KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
                                 vertegenwoordiger.VertegenwoordigerId,
                                 vertegenwoordiger.Insz,
                                 vertegenwoordiger.Voornaam,
                                 vertegenwoordiger.Achternaam));
                else
                {
                    AddEvent(new KszSyncHeeftVertegenwoordigerBevestigd(vertegenwoordiger.VertegenwoordigerId));
                }

            }
            catch (EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz e)
            {
                AddEvent(new KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(vertegenwoordiger.VertegenwoordigerId,
                                                                                 vertegenwoordiger.Insz,
                                                                                 vertegenwoordiger.Voornaam,
                                                                                 vertegenwoordiger.Achternaam));
            }
        }
    }

    public void MarkeerVertegenwoordigerAlsOverleden(int vertegenwoordigerId)
    {
        var vertegenwoordiger = State.Vertegenwoordigers.SingleOrDefault(x => x.VertegenwoordigerId == vertegenwoordigerId);
        if(vertegenwoordiger == null)
            return;

        AddEvent(new KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
                     vertegenwoordiger.VertegenwoordigerId,
                     vertegenwoordiger.Insz,
                     vertegenwoordiger.Voornaam,
                     vertegenwoordiger.Achternaam));
    }

    public int VoegBankrekeningToe(ToeTevoegenBankrekeningnummer bankrekeningnummer)
    {
        var toegevoegdBankrekeningnummer = State.Bankrekeningnummers.VoegToe(bankrekeningnummer);

        AddEvent(new BankrekeningnummerWerdToegevoegd(toegevoegdBankrekeningnummer.Id, toegevoegdBankrekeningnummer.Iban.Value, toegevoegdBankrekeningnummer.GebruiktVoor, toegevoegdBankrekeningnummer.Titularis));

        return toegevoegdBankrekeningnummer.Id;
    }
}
