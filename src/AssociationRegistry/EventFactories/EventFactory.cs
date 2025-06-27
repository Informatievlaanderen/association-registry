namespace AssociationRegistry.EventFactories;

using Events;
using GemeentenaamVerrijking;
using Grar.Models;
using Kbo;
using Vereniging;
using Vereniging.Geotags;

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
        => new(EventFactory.Locatie(locatie));

    public static Registratiedata.Locatie Locatie(Locatie locatie)
        => new(
            locatie.LocatieId,
            locatie.Locatietype,
            locatie.IsPrimair,
            locatie.Naam ?? string.Empty,
            EventFactory.Adres(locatie.Adres),
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

    public static WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(
        VCode vCode,
        VerenigingStatus.StatusDubbel verenigingStatus)
        => new(vCode, verenigingStatus.VCodeAuthentiekeVereniging, verenigingStatus.VorigeVerenigingStatus.StatusNaam);

    public static WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald(
        VCode vCode,
        IEnumerable<Werkingsgebied> werkingsgebieden)
        => new(vCode, werkingsgebieden
                     .Select(EventFactory.Werkingsgebied)
                     .ToArray());

    public static WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd(VCode vCode, IEnumerable<Werkingsgebied> werkingsgebieden)
        => new(vCode, werkingsgebieden
                     .Select(EventFactory.Werkingsgebied)
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
        => new(EventFactory.Locatie(locatie));

    public static MaatschappelijkeZetelWerdGewijzigdInKbo MaatschappelijkeZetelWerdGewijzigdInKbo(Locatie locatie)
        => new(EventFactory.Locatie(locatie));

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
        => new(vCode, EventFactory.Locatie(locatie));

    public static LocatieWerdToegevoegd LocatieWerdToegevoegd(Locatie locatie)
        => new(EventFactory.Locatie(locatie));

    public static LocatieWerdGewijzigd LocatieWerdGewijzigd(Locatie locatie)
        => new(EventFactory.Locatie(locatie));

    public static LidmaatschapWerdVerwijderd LidmaatschapWerdVerwijderd(VCode vCode, Lidmaatschap lidmaatschap)
        => new(vCode, EventFactory.Lidmaatschap(lidmaatschap));

    public static LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd(VCode vCode, Lidmaatschap lidmaatschap)
        => new(
            vCode,
            EventFactory.Lidmaatschap(lidmaatschap)
        );

    public static LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd(VCode vCode, Lidmaatschap lidmaatschap)
        => new(
            vCode,
            EventFactory.Lidmaatschap(lidmaatschap)
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
        => new(EventFactory.Doelgroep(doelgroep));

    public static HoofdactiviteitenVerenigingsloketWerdenGewijzigd HoofdactiviteitenVerenigingsloketWerdenGewijzigd(
        IEnumerable<HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket)
        => new(hoofdactiviteitenVerenigingsloket
              .Select(EventFactory.HoofdactiviteitVerenigingsloket)
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

    public static Registratiedata.AdresUitAdressenregister AdresUitAdressenregister(
        IAddressResponse adres,
        VerrijkteGemeentenaam gemeentenaam)
        => new(
            adres.Straatnaam,
            adres.Huisnummer,
            adres.Busnummer,
            adres.Postcode,
            gemeentenaam.Format());

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
}
