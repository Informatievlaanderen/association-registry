namespace AssociationRegistry.Events.Factories;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Events;
using AssociationRegistry.GemeentenaamVerrijking;
using AssociationRegistry.Grar.Models;
using Be.Vlaanderen.Basisregisters.Utilities;
using DecentraalBeheer.Vereniging.Adressen.GemeentenaamVerrijking;
using DecentraalBeheer.Vereniging.DubbelDetectie;
using Grar.AdresMatch;
using Magda.Kbo;

public static class EventFactory
{
    public static ContactgegevenWerdGewijzigd ContactgegevenWerdGewijzigd(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            contactgegeven.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);

    public static ContactgegevenUitKBOWerdGewijzigd ContactgegevenUitKBOWerdGewijzigd(Contactgegeven contactgegeven)
        => new(contactgegeven.ContactgegevenId, contactgegeven.Beschrijving, contactgegeven.IsPrimair);

    public static AdresWerdNietGevondenInAdressenregister AdresWerdNietGevondenInAdressenregister(VCode vCode, Locatie locatie)
        => new(vCode, locatie.LocatieId, locatie.Adres.Straatnaam, locatie.Adres.Huisnummer,
               locatie.Adres.Busnummer, locatie.Adres.Postcode, locatie.Adres.Gemeente.Naam);

    public static ContactgegevenWerdGewijzigdInKbo ContactgegevenWerdGewijzigdInKbo(
        Contactgegeven contactgegeven,
        ContactgegeventypeVolgensKbo typeVolgensKbo)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            typeVolgensKbo.Waarde,
            contactgegeven.Waarde);

    public static MaatschappelijkeZetelWerdVerwijderdUitKbo MaatschappelijkeZetelWerdVerwijderdUitKbo(Locatie locatie)
        => new(Locatie(locatie));

    public static Registratiedata.Locatie Locatie(Locatie locatie)
        => new(
            locatie.LocatieId,
            locatie.Locatietype,
            locatie.IsPrimair,
            locatie.Naam ?? string.Empty,
            Adres(locatie.Adres),
            locatie.AdresId is not null ? new Registratiedata.AdresId(locatie.AdresId.Adresbron.Code, locatie.AdresId.Bronwaarde) : null);

    public static Registratiedata.Contactgegeven Contactgegeven(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            contactgegeven.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);

    public static VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging(VCode vCode, VCode dubbeleVereniging)
        => new(vCode, dubbeleVereniging);

    public static VerenigingWerdGestoptInKBO VerenigingWerdGestoptInKBO(Datum einddatum)
        => new(einddatum.Value);

    public static VerenigingWerdGestopt VerenigingWerdGestopt(Datum einddatum)
        => new(einddatum.Value);

    public static VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan(VCode vCode, VCode vCodeAuthentiekeVereniging)
        => new(vCode, vCodeAuthentiekeVereniging);

    public static VerenigingAanvaarddeCorrectieDubbeleVereniging VerenigingAanvaarddeCorrectieDubbeleVereniging(
        VCode vCode,
        VCode dubbeleVereniging)
        => new(vCode, dubbeleVereniging);

    public static StartdatumWerdGewijzigd StartdatumWerdGewijzigd(VCode vCode, Datum? startDatum)
        => new(vCode, startDatum?.Value ?? null);

    public static StartdatumWerdGewijzigdInKbo StartdatumWerdGewijzigdInKbo(Datum? startDatum)
        => new(startDatum?.Value ?? null);

    public static VertegenwoordigerWerdGewijzigd VertegenwoordigerWerdGewijzigd(Vertegenwoordiger vertegenwoordiger)
        => new(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.IsPrimair,
            vertegenwoordiger.Roepnaam ?? string.Empty,
            vertegenwoordiger.Rol ?? string.Empty,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            vertegenwoordiger.Email.Waarde,
            vertegenwoordiger.Telefoon.Waarde,
            vertegenwoordiger.Mobiel.Waarde,
            vertegenwoordiger.SocialMedia.Waarde
        );

    public static WerkingsgebiedenWerdenNietBepaald WerkingsgebiedenWerdenNietBepaald(VCode vCode) => new(vCode);
    public static WerkingsgebiedenWerdenNietVanToepassing WerkingsgebiedenWerdenNietVanToepassing(VCode vCode) => new(vCode);

    public static VertegenwoordigerWerdVerwijderd VertegenwoordigerWerdVerwijderd(Vertegenwoordiger vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId, vertegenwoordiger.Insz, vertegenwoordiger.Voornaam, vertegenwoordiger.Achternaam);

    public static VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKbo(Vertegenwoordiger vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId, vertegenwoordiger.Insz, vertegenwoordiger.Voornaam, vertegenwoordiger.Achternaam);

    public static VertegenwoordigerWerdGewijzigdInKBO VertegenwoordigerWerdGewijzigdInKBO(Vertegenwoordiger vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId, vertegenwoordiger.Insz, vertegenwoordiger.Voornaam, vertegenwoordiger.Achternaam);

    public static VertegenwoordigerWerdVerwijderdUitKBO VertegenwoordigerWerdVerwijderdUitKBO(Vertegenwoordiger vertegenwoordiger)
        => new(vertegenwoordiger.VertegenwoordigerId, vertegenwoordiger.Insz, vertegenwoordiger.Voornaam, vertegenwoordiger.Achternaam);

    public static WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(
        VCode vCode,
        VerenigingStatus.StatusDubbel verenigingStatus)
        => new(vCode, verenigingStatus.VCodeAuthentiekeVereniging, verenigingStatus.VorigeVerenigingStatus.StatusNaam);

    public static WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald(
        VCode vCode,
        IEnumerable<Werkingsgebied> werkingsgebieden)
        => new(vCode, werkingsgebieden
                     .Select(Werkingsgebied)
                     .ToArray());

    public static WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd(VCode vCode, IEnumerable<Werkingsgebied> werkingsgebieden)
        => new(vCode, werkingsgebieden
                     .Select(Werkingsgebied)
                     .ToArray());

    public static VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd(Vertegenwoordiger vertegenwoordiger)
        => new(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.Insz,
            vertegenwoordiger.IsPrimair,
            vertegenwoordiger.Roepnaam ?? string.Empty,
            vertegenwoordiger.Rol ?? string.Empty,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            vertegenwoordiger.Email.Waarde,
            vertegenwoordiger.Telefoon.Waarde,
            vertegenwoordiger.Mobiel.Waarde,
            vertegenwoordiger.SocialMedia.Waarde
        );

    public static Registratiedata.Adres? Adres(Adres? adres)
    {
        if (adres is null)
            return null;

        return new Registratiedata.Adres(
            adres.Straatnaam,
            adres.Huisnummer,
            adres.Busnummer,
            adres.Postcode,
            adres.Gemeente.Naam,
            adres.Land);
    }

    public static Registratiedata.AdresId? AdresId(AdresId? adresId)
    {
        if (adresId is null)
            return null;

        return new Registratiedata.AdresId(
            adresId.Adresbron.Code,
            adresId.Bronwaarde);
    }

    public static Registratiedata.HoofdactiviteitVerenigingsloket HoofdactiviteitVerenigingsloket(
        HoofdactiviteitVerenigingsloket activiteit)
        => new(activiteit.Code, activiteit.Naam);

    public static MarkeringDubbeleVerengingWerdGecorrigeerd MarkeringDubbeleVerengingWerdGecorrigeerd(
        VCode vCode,
        VerenigingStatus.StatusDubbel verenigingStatus)
        => new(vCode, verenigingStatus.VCodeAuthentiekeVereniging, verenigingStatus.VorigeVerenigingStatus.StatusNaam);

    public static Registratiedata.Werkingsgebied Werkingsgebied(Werkingsgebied werkingsgebied)
        => new(werkingsgebied.Code, werkingsgebied.Naam);

    public static RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO(Verenigingstype verenigingstype)
        => new(verenigingstype.Code);

    public static Registratiedata.Doelgroep Doelgroep(Doelgroep doelgroep)
        => new(doelgroep.Minimumleeftijd, doelgroep.Maximumleeftijd);

    public static Registratiedata.Lidmaatschap Lidmaatschap(Lidmaatschap lidmaatschap)
        => new(
            lidmaatschap.LidmaatschapId,
            lidmaatschap.AndereVereniging,
            lidmaatschap.AndereVerenigingNaam,
            lidmaatschap.Geldigheidsperiode.Van.DateOnly,
            lidmaatschap.Geldigheidsperiode.Tot.DateOnly,
            lidmaatschap.Identificatie,
            lidmaatschap.Beschrijving);

    public static Registratiedata.Vertegenwoordiger Vertegenwoordiger(Vertegenwoordiger vertegenwoordiger)
        => new(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.Insz,
            vertegenwoordiger.IsPrimair,
            vertegenwoordiger.Roepnaam ?? string.Empty,
            vertegenwoordiger.Rol ?? string.Empty,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            vertegenwoordiger.Email.Waarde,
            vertegenwoordiger.Telefoon.Waarde,
            vertegenwoordiger.Mobiel.Waarde,
            vertegenwoordiger.SocialMedia.Waarde);

    public static ContactgegevenWerdInBeheerGenomenDoorKbo ContactgegevenWerdInBeheerGenomenDoorKbo(
        Contactgegeven contactgegeven,
        ContactgegeventypeVolgensKbo typeVolgensKbo)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            typeVolgensKbo.Waarde,
            contactgegeven.Waarde);

    public static MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo(Locatie locatie)
        => new(Locatie(locatie));

    public static MaatschappelijkeZetelWerdGewijzigdInKbo MaatschappelijkeZetelWerdGewijzigdInKbo(Locatie locatie)
        => new(Locatie(locatie));

    public static MaatschappelijkeZetelVolgensKBOWerdGewijzigd MaatschappelijkeZetelVolgensKBOWerdGewijzigd(Locatie locatie)
        => new(locatie.LocatieId, locatie.Naam ?? string.Empty, locatie.IsPrimair);

    public static MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
        AdresVolgensKbo adres)
        => new(
            adres.Straatnaam ?? string.Empty,
            adres.Huisnummer ?? string.Empty,
            adres.Busnummer ?? string.Empty,
            adres.Postcode ?? string.Empty,
            adres.Gemeente ?? string.Empty,
            adres.Land ?? string.Empty);

    public static LocatieWerdVerwijderd LocatieWerdVerwijderd(VCode vCode, Locatie locatie)
        => new(vCode, Locatie(locatie));

    public static LocatieWerdToegevoegd LocatieWerdToegevoegd(Locatie locatie)
        => new(Locatie(locatie));

    public static LocatieWerdGewijzigd LocatieWerdGewijzigd(Locatie locatie)
        => new(Locatie(locatie));

    public static LidmaatschapWerdVerwijderd LidmaatschapWerdVerwijderd(VCode vCode, Lidmaatschap lidmaatschap)
        => new(vCode, Lidmaatschap(lidmaatschap));

    public static LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd(VCode vCode, Lidmaatschap lidmaatschap)
        => new(
            vCode,
            Lidmaatschap(lidmaatschap)
        );

    public static LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd(VCode vCode, Lidmaatschap lidmaatschap)
        => new(
            vCode,
            Lidmaatschap(lidmaatschap)
        );

    public static NietUniekeAdresMatchUitAdressenregister NietUniekeAdresMatchUitAdressenregister(AddressMatchResponse response)
        => new()
        {
            Score = response.Score,
            AdresId = response.AdresId,
            Adresvoorstelling = response.Adresvoorstelling,
        };

    public static ContactgegevenWerdVerwijderd ContactgegevenWerdVerwijderd(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            contactgegeven.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);

    public static DoelgroepWerdGewijzigd DoelgroepWerdGewijzigd(Doelgroep doelgroep)
        => new(Doelgroep(doelgroep));

    public static HoofdactiviteitenVerenigingsloketWerdenGewijzigd HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
        IEnumerable<HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket)
        => new(hoofdactiviteitenVerenigingsloket
              .Select(HoofdactiviteitVerenigingsloket)
              .ToArray());

    public static EinddatumWerdGewijzigd EinddatumWerdGewijzigd(Datum datum)
        => new(datum.Value);

    public static ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            contactgegeven.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);

    public static ContactgegevenWerdVerwijderdUitKBO ContactgegevenWerdVerwijderdUitKBO(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            contactgegeven.TypeVolgensKbo!.Waarde,
            contactgegeven.Waarde);

    public static ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKBO(
        Contactgegeven contactgegeven,
        ContactgegeventypeVolgensKbo typeVolgensKbo)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            typeVolgensKbo.Waarde,
            contactgegeven.Waarde);

    public static AdresDetailUitAdressenregister AdresDetailUitAdressenregister(AddressDetailResponse response)
        => new()
        {
            AdresId = response.AdresId,
            Adres = new Registratiedata.AdresUitAdressenregister(
                response.Straatnaam,
                response.Huisnummer,
                response.Busnummer,
                response.Postcode,
                response.Gemeente),
        };

    public static AdresDetailUitAdressenregister VerrijktAdresUitAdressenregister(
        VerrijktAdresUitGrar verrijktAdresUitGrar)
        => new()
        {
            AdresId = verrijktAdresUitGrar.AddressResponse.AdresId,
            Adres = new Registratiedata.AdresUitAdressenregister(
                verrijktAdresUitGrar.AddressResponse.Straatnaam,
                verrijktAdresUitGrar.AddressResponse.Huisnummer,
                verrijktAdresUitGrar.AddressResponse.Busnummer,
                verrijktAdresUitGrar.AddressResponse.Postcode,
                verrijktAdresUitGrar.Gemeente.Naam),
        };

    public static Registratiedata.AdresUitAdressenregister FromVerrijktAdresUitAdressenregister(
        VerrijktAdresUitGrar verrijktAdresUitGrar)
        => new(
            verrijktAdresUitGrar.AddressResponse.Straatnaam,
            verrijktAdresUitGrar.AddressResponse.Huisnummer,
            verrijktAdresUitGrar.AddressResponse.Busnummer,
            verrijktAdresUitGrar.AddressResponse.Postcode,
            verrijktAdresUitGrar.Gemeente.Naam);

    public static Registratiedata.AdresUitAdressenregister? AdresUitAdressenregister(AdresDetailUitAdressenregister? adres)
    {
        if (adres is null)
            return null;

        return new Registratiedata.AdresUitAdressenregister(
            adres.Adres.Straatnaam,
            adres.Adres.Huisnummer,
            adres.Adres.Busnummer,
            adres.Adres.Postcode,
            adres.Adres.Gemeente);
    }

    public static VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging SubtypeWerdVerfijndNaarFeitelijkeVereniging(VCode vCode)
        => new(vCode);

    public static VerenigingssubtypeWerdTerugGezetNaarNietBepaald SubtypeWerdTerugGezetNaarNietBepaald(VCode vCode)
        => new(vCode);

    public static VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging(
        VCode vCode,
        string andereVereniging,
        string andereVerenigingNaam,
        string identificatie,
        string beschrijving)
        => new(vCode,
               new Registratiedata.SubverenigingVan(andereVereniging,
                                                    andereVerenigingNaam,
                                                    identificatie,
                                                    beschrijving));

    public static SubverenigingRelatieWerdGewijzigd SubverenigingRelatieWerdGewijzigd(
        VCode vCode,
        string andereVereniging,
        string andereVerenigingNaam)
        => new(vCode,
               andereVereniging,
               andereVerenigingNaam);

    public static SubverenigingDetailsWerdenGewijzigd DetailGegevensVanDeSubverenigingRelatieWerdenGewijzigd(
        VCode vCode,
        string identificatie,
        string beschrijving)
        => new(vCode,
               identificatie,
               beschrijving);

    public static GeotagsWerdenBepaald GeotagsWerdenBepaald(VCode vCode, GeotagsCollection geotags)
    => new(vCode, geotags.Select(x => new Registratiedata.Geotag(x.Identificatie)).ToArray());

    public static DubbeleVerenigingenWerdenGedetecteerd DubbeleVerenigingenWerdenGedetecteerd(
        string bevestigingstoken,
        string naam,
        Locatie[] locaties,
        DuplicaatVereniging[] gedetecteerdeDubbels)
        => new(bevestigingstoken,
               naam,
               locaties.Select(Locatie).ToArray(),
               gedetecteerdeDubbels.Select(DuplicaatVereniging).ToArray());

    private static Registratiedata.DuplicateVereniging DuplicaatVereniging(DuplicaatVereniging duplicaatVereniging)
        => new(
            duplicaatVereniging.VCode,
            Verenigingstype(duplicaatVereniging.Verenigingstype),
            Verenigingssubtype(duplicaatVereniging.Verenigingssubtype),
            duplicaatVereniging.Naam,
            duplicaatVereniging.KorteNaam,
            duplicaatVereniging.HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket).ToArray(),
            duplicaatVereniging.Locaties.Select(Locatie).ToArray()
        );

    private static Registratiedata.HoofdactiviteitVerenigingsloket HoofdactiviteitVerenigingsloket(
        DuplicaatVereniging.Types.HoofdactiviteitVerenigingsloket activiteit)
        => new(activiteit.Code, activiteit.Naam);
    private static Registratiedata.DuplicateVerenigingLocatie Locatie(
        DuplicaatVereniging.Types.Locatie locatie)
        => new(
            locatie.Locatietype,
            locatie.IsPrimair,
            locatie.Adres,
            locatie.Naam,
            locatie.Postcode,
            locatie.Gemeente);

    private static Registratiedata.Verenigingstype Verenigingstype(DuplicaatVereniging.Types.Verenigingstype verenigingstype)
        => new(verenigingstype.Code, verenigingstype.Naam);
    private static Registratiedata.Verenigingssubtype? Verenigingssubtype(DuplicaatVereniging.Types.Verenigingssubtype? verenigingssubtype)
        => verenigingssubtype is null ? null : new Registratiedata.Verenigingssubtype(verenigingssubtype.Code, verenigingssubtype.Naam);
}
