namespace AssociationRegistry.Test.Projections.Beheer.Detail.Dubbels;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Events;
using Formats;
using Vereniging.Bronnen;

/// <summary>
///     Hoe een overgenomen gegeven uit de samenvoeging er op het beheer detail document moet uitzien.
///     Veld per veld uitgeschreven, zodat de test de mapping van de projectie controleert in plaats van ze te herhalen.
/// </summary>
public static class ExpectedBeheerDetail
{
    public static Doelgroep Doelgroep(Registratiedata.Doelgroep doelgroep, string vCode) =>
        new()
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Doelgroep, vCode),
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };

    public static HoofdactiviteitVerenigingsloket Hoofdactiviteit(
        Registratiedata.HoofdactiviteitVerenigingsloket hoofdactiviteit
    ) =>
        new()
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Hoofdactiviteit,
                hoofdactiviteit.Code
            ),
            Code = hoofdactiviteit.Code,
            Naam = hoofdactiviteit.Naam,
        };

    public static Werkingsgebied Werkingsgebied(Registratiedata.Werkingsgebied werkingsgebied) =>
        new()
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Werkingsgebied,
                werkingsgebied.Code
            ),
            Code = werkingsgebied.Code,
            Naam = werkingsgebied.Naam,
        };

    public static Contactgegeven Contactgegeven(Registratiedata.Contactgegeven contactgegeven, string vCode) =>
        new()
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Contactgegeven,
                vCode,
                contactgegeven.ContactgegevenId.ToString()
            ),
            ContactgegevenId = contactgegeven.ContactgegevenId,
            Contactgegeventype = contactgegeven.Contactgegeventype,
            Waarde = contactgegeven.Waarde,
            Beschrijving = contactgegeven.Beschrijving,
            IsPrimair = contactgegeven.IsPrimair,
            Bron = Bron.Initiator,
        };

    public static Locatie Locatie(Registratiedata.Locatie locatie, string vCode)
    {
        var locatieId = locatie.LocatieId.ToString();

        return new Locatie
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Locatie, vCode, locatieId),
            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            IsPrimair = locatie.IsPrimair,
            Naam = locatie.Naam,
            Adres = new Adres
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Adres, vCode, locatieId),
                Straatnaam = locatie.Adres!.Straatnaam,
                Huisnummer = locatie.Adres.Huisnummer,
                Busnummer = locatie.Adres.Busnummer,
                Postcode = locatie.Adres.Postcode,
                Gemeente = locatie.Adres.Gemeente,
                Land = locatie.Adres.Land,
            },
            Adresvoorstelling = locatie.Adres.ToAdresString(),
            AdresId = new AdresId { Broncode = locatie.AdresId!.Broncode, Bronwaarde = locatie.AdresId.Bronwaarde },
            VerwijstNaar = new AdresVerwijzing
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.AdresVerwijzing,
                    locatie.AdresId.Bronwaarde.Split('/').Last()
                ),
            },
            Bron = Bron.Initiator,
        };
    }

    public static Vertegenwoordiger Vertegenwoordiger(Registratiedata.Vertegenwoordiger vertegenwoordiger, string vCode)
    {
        var vertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId.ToString();

        return new Vertegenwoordiger
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Vertegenwoordiger,
                vCode,
                vertegenwoordigerId
            ),
            VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
            Insz = vertegenwoordiger.Insz,
            IsPrimair = vertegenwoordiger.IsPrimair,
            Roepnaam = vertegenwoordiger.Roepnaam,
            Rol = vertegenwoordiger.Rol,
            Voornaam = vertegenwoordiger.Voornaam,
            Achternaam = vertegenwoordiger.Achternaam,
            Email = vertegenwoordiger.Email,
            Telefoon = vertegenwoordiger.Telefoon,
            Mobiel = vertegenwoordiger.Mobiel,
            SocialMedia = vertegenwoordiger.SocialMedia,
            VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.VertegenwoordigerContactgegeven,
                    vCode,
                    vertegenwoordigerId
                ),
                IsPrimair = vertegenwoordiger.IsPrimair,
                Email = vertegenwoordiger.Email,
                Telefoon = vertegenwoordiger.Telefoon,
                Mobiel = vertegenwoordiger.Mobiel,
                SocialMedia = vertegenwoordiger.SocialMedia,
            },
            Bron = Bron.Initiator,
        };
    }

    public static Bankrekeningnummer Bankrekeningnummer(
        Registratiedata.Bankrekeningnummer bankrekeningnummer,
        string vCode
    ) =>
        new()
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Bankrekeningnummer,
                vCode,
                bankrekeningnummer.BankrekeningnummerId.ToString()
            ),
            BankrekeningnummerId = bankrekeningnummer.BankrekeningnummerId,
            Iban = bankrekeningnummer.Iban,
            Doel = bankrekeningnummer.Doel,
            Titularissen = bankrekeningnummer.Titularissen,
            BevestigdDoor = [],
            Bron = Bron.Initiator,
        };

    public static Erkenning Erkenning(SamenvoegingDubbeleVereniging.Erkenning erkenning, string vCode) =>
        new()
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Erkenning,
                vCode,
                erkenning.ErkenningId.ToString()
            ),
            ErkenningId = erkenning.ErkenningId,
            GeregistreerdDoor = new GegevensInitiator
            {
                OvoCode = erkenning.GeregistreerdDoor.OvoCode,
                Naam = erkenning.GeregistreerdDoor.Naam,
            },
            IpdcProduct = new IpdcProduct { Nummer = erkenning.IpdcProduct.Nummer, Naam = erkenning.IpdcProduct.Naam },
            Startdatum = erkenning.Startdatum?.ToString(WellknownFormats.DateOnly),
            Einddatum = erkenning.Einddatum?.ToString(WellknownFormats.DateOnly),
            Hernieuwingsdatum = erkenning.Hernieuwingsdatum?.ToString(WellknownFormats.DateOnly),
            HernieuwingsUrl = erkenning.HernieuwingsUrl,
            RedenSchorsing = erkenning.RedenSchorsing,
            Status = erkenning.Status,
        };

    public static Lidmaatschap Lidmaatschap(Registratiedata.Lidmaatschap lidmaatschap, string vCode) =>
        new(
            BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Lidmaatschap,
                vCode,
                lidmaatschap.LidmaatschapId.ToString()
            ),
            lidmaatschap.LidmaatschapId,
            lidmaatschap.AndereVereniging,
            lidmaatschap.DatumVan,
            lidmaatschap.DatumTot,
            lidmaatschap.Identificatie,
            lidmaatschap.Beschrijving
        );

    public static SubverenigingVan SubverenigingVan(Registratiedata.SubverenigingVan subverenigingVan) =>
        new()
        {
            AndereVereniging = subverenigingVan.AndereVereniging,
            AndereVerenigingNaam = subverenigingVan.AndereVerenigingNaam,
            Identificatie = subverenigingVan.Identificatie,
            Beschrijving = subverenigingVan.Beschrijving,
        };
}
