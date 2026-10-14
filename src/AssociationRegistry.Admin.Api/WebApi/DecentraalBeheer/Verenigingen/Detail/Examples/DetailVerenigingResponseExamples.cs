namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.Examples;

using AssociationRegistry.Formats;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Bronnen;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Mappers;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using Verenigingstype = ResponseModels.Verenigingstype;
using Vertegenwoordiger = ResponseModels.Vertegenwoordiger;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class DetailVerenigingResponseExamples : IMultipleExamplesProvider<DetailVerenigingResponse>
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public IEnumerable<SwaggerExample<DetailVerenigingResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Zonder versie header",
            DetailResponse(
                VrolijkeBas(type: DecentraalBeheer.Vereniging.Verenigingstype.FeitelijkeVereniging, subtype: null)
            )
        );

        yield return SwaggerExample.Create(
            name: "Versie 'v2'",
            DetailResponse(
                VrolijkeBas(
                    type: DecentraalBeheer.Vereniging.Verenigingstype.VZER,
                    subtype: VerenigingssubtypeCode.NietBepaald.Map<Verenigingssubtype>()
                )
            )
        );

        yield return SwaggerExample.Create(
            name: "Vereniging met rechtspersoonlijkheid",
            DetailResponse(VissenAchterHetNet())
        );
    }

    private DetailVerenigingResponse DetailResponse(VerenigingDetail vereniging) =>
        new()
        {
            Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/detail-vereniging-context.json",
            Vereniging = vereniging,
            Metadata = new Metadata { DatumLaatsteAanpassing = "2020-05-15" },
        };

    // Varianten 1 en 2 delen alles behalve verenigingstype/subtype.
    private static VerenigingDetail VrolijkeBas(
        DecentraalBeheer.Vereniging.Verenigingstype type,
        Verenigingssubtype? subtype
    ) =>
        new()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            VCode = "V0001001",
            Verenigingstype = new Verenigingstype { Naam = type.Naam, Code = type.Code },
            Verenigingssubtype = subtype,
            Naam = "FWA De vrolijke BA’s",
            KorteNaam = "DVB",
            KorteBeschrijving = "De vereniging van de vrolijke BA's",
            Startdatum = "2020-05-15",
            Einddatum = "2021-12-12",
            Doelgroep = DoelgroepMetId(min: 0, max: 150),
            Status = VerenigingStatus.Gestopt,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            HoofdactiviteitenVerenigingsloket = new[] { HoofdactiviteitCult() },
            Werkingsgebieden = new[] { WerkingsgebiedWestVlaanderen() },
            Contactgegevens = new[] { InfoEmail(Bron.Initiator.Waarde) },
            Locaties = new[] { AdministratiefCentrumLiedekerke() },
            Vertegenwoordigers = new[] { JaneDoo() },
            Sleutels = new[] { VrSleutel() },
            Relaties = Array.Empty<Relatie>(),
            Lidmaatschappen = new[] { LidmaatschapVasteDatums() },
            Bankrekeningnummers = new[] { LidgeldBankrekening() },
            IsDubbelVan = "",
            Bron = Bron.Initiator.Waarde,
        };

    private static VerenigingDetail VissenAchterHetNet() =>
        new()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            VCode = "V0001002",
            Verenigingstype = new Verenigingstype
            {
                Naam = DecentraalBeheer.Vereniging.Verenigingstype.VZW.Naam,
                Code = DecentraalBeheer.Vereniging.Verenigingstype.VZW.Code,
            },
            Naam = "Vissen achter het net",
            KorteNaam = "VAN",
            Roepnaam = "Vissen achter 't net",
            KorteBeschrijving = "Een kleine groep vissers",
            Startdatum = "2002-11-15",
            Einddatum = null,
            Doelgroep = new DoelgroepResponse { Minimumleeftijd = 60, Maximumleeftijd = 90 },
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            HoofdactiviteitenVerenigingsloket = new[] { HoofdactiviteitCult() },
            Contactgegevens = new[] { InfoEmail(Bron.KBO.Waarde), BlubBlubSocialMedia() },
            Locaties = new[] { ZeebankOostende(), VisPlaatsWestende() },
            Vertegenwoordigers = Array.Empty<Vertegenwoordiger>(),
            Sleutels = new[] { VrSleutel(), KboSleutel() },
            Relaties = Array.Empty<Relatie>(),
            Lidmaatschappen = new[] { GewoonLidRelatief(), TijdelijkLidRelatief() },
            Bankrekeningnummers = new[] { LidgeldBankrekening() },
            IsDubbelVan = "",
            Bron = Bron.KBO.Waarde,
        };

    private static DoelgroepResponse DoelgroepMetId(int min, int max) =>
        new()
        {
            id = JsonLdType.Doelgroep.CreateWithIdValues("V0001001"),
            type = JsonLdType.Doelgroep.Type,
            Minimumleeftijd = min,
            Maximumleeftijd = max,
        };

    private static HoofdactiviteitVerenigingsloket HoofdactiviteitCult() =>
        new()
        {
            id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("CULT"),
            type = JsonLdType.Hoofdactiviteit.Type,
            Code = "CULT",
            Naam = "Cultuur",
        };

    private static Werkingsgebied WerkingsgebiedWestVlaanderen() =>
        new()
        {
            id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
            type = JsonLdType.Werkingsgebied.Type,
            Code = "BE25",
            Naam = "Provincie West-Vlaanderen",
        };

    private static Contactgegeven InfoEmail(string bron) =>
        new()
        {
            id = JsonLdType.Contactgegeven.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Contactgegeven.Type,
            Contactgegeventype = "E-mail",
            Beschrijving = "Info",
            Waarde = "info@example.org",
            ContactgegevenId = 1,
            IsPrimair = false,
            Bron = bron,
        };

    private static Contactgegeven BlubBlubSocialMedia() =>
        new()
        {
            id = JsonLdType.Contactgegeven.CreateWithIdValues("V0001001", "2"),
            type = JsonLdType.Contactgegeven.Type,
            Contactgegeventype = Contactgegeventype.SocialMedia,
            Beschrijving = "BlubBlub",
            Waarde = "blubblub.com/vissen",
            ContactgegevenId = 2,
            IsPrimair = false,
            Bron = Bron.Initiator.Waarde,
        };

    private static Locatie AdministratiefCentrumLiedekerke() =>
        new()
        {
            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Locatie.Type,
            LocatieId = 1,
            Locatietype = Locatietype.Correspondentie.Waarde,
            IsPrimair = true,
            Adresvoorstelling = "Kerkstraat 5, 1770 Liedekerke, België",
            Naam = "Administratief centrum",
            Adres = AdresMet("1", "Kerkstraat", "5", "b", "1770", "Liedekerke"),
            AdresId = ArAdresId(suffix: 1),
            VerwijstNaar = AdresVerwijzing("1"),
            Bron = Bron.Initiator,
        };

    private static Locatie ZeebankOostende() =>
        new()
        {
            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Locatie.Type,
            LocatieId = 1,
            Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
            IsPrimair = true,
            Adresvoorstelling = "Zeebank 10, 8400 Oostende, België",
            Naam = "",
            Adres = AdresMet("1", "Zeebank", "10", "", "8400", "Oostende"),
            Bron = Bron.KBO.Waarde,
        };

    private static Locatie VisPlaatsWestende() =>
        new()
        {
            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "2"),
            type = JsonLdType.Locatie.Type,
            LocatieId = 2,
            Locatietype = Locatietype.Activiteiten.Waarde,
            IsPrimair = false,
            Adresvoorstelling = "De pier 1, 8430 Westende",
            Naam = "Vis plaats",
            Adres = AdresMet("2", "De pier", "1", "", "8430", "Westende"),
            AdresId = ArAdresId(suffix: 17),
            VerwijstNaar = AdresVerwijzing("17"),
            Bron = Bron.Initiator.Waarde,
        };

    private static Adres AdresMet(
        string locatieId,
        string straat,
        string huisnummer,
        string bus,
        string postcode,
        string gemeente
    ) =>
        new()
        {
            id = JsonLdType.Adres.CreateWithIdValues("V0001001", locatieId),
            type = JsonLdType.Adres.Type,
            Straatnaam = straat,
            Huisnummer = huisnummer,
            Busnummer = bus,
            Postcode = postcode,
            Gemeente = gemeente,
            Land = "België",
        };

    private static AdresId ArAdresId(int suffix) =>
        new()
        {
            Broncode = Adresbron.AR,
            Bronwaarde = DecentraalBeheer.Vereniging.Adressen.AdresId.DataVlaanderenAdresPrefix + suffix,
        };

    private static AdresVerwijzing AdresVerwijzing(string id) =>
        new() { id = JsonLdType.AdresVerwijzing.CreateWithIdValues(id), type = JsonLdType.AdresVerwijzing.Type };

    private static Vertegenwoordiger JaneDoo() =>
        new()
        {
            id = JsonLdType.Vertegenwoordiger.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Vertegenwoordiger.Type,
            VertegenwoordigerId = 1,
            Insz = "1234567890",
            Voornaam = "Jane",
            Achternaam = "Doo",
            PrimairContactpersoon = false,
            Roepnaam = "Jhony",
            Rol = "Voorzitter",
            Email = "jhon@example.org",
            Mobiel = "0000112233",
            Telefoon = "0001112233",
            SocialMedia = "http://example.org",
            VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
            {
                id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues("V0001001", "1"),
                type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                Email = "jhon@example.org",
                Mobiel = "0000112233",
                Telefoon = "0001112233",
                SocialMedia = "http://example.org",
                IsPrimair = false,
            },
            Bron = Bron.Initiator.Waarde,
        };

    private static Sleutel VrSleutel() =>
        new()
        {
            id = JsonLdType.Sleutel.CreateWithIdValues("V0001001", Sleutelbron.VR.Waarde),
            type = JsonLdType.Sleutel.Type,
            Waarde = "V0001001",
            Bron = Sleutelbron.VR.Waarde,
            CodeerSysteem = CodeerSysteem.VR.Waarde,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0001001", Sleutelbron.VR.Waarde),
                type = JsonLdType.GestructureerdeSleutel.Type,
                Nummer = "V0001001",
            },
        };

    private static Sleutel KboSleutel() =>
        new()
        {
            id = JsonLdType.Sleutel.CreateWithIdValues("V0001001", Sleutelbron.KBO.Waarde),
            type = JsonLdType.Sleutel.Type,
            Waarde = "0123456789",
            Bron = Sleutelbron.KBO.Waarde,
            CodeerSysteem = CodeerSysteem.KBO.Waarde,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0001001", Sleutelbron.KBO.Waarde),
                type = JsonLdType.GestructureerdeSleutel.Type,
                Nummer = "0123456789",
            },
        };

    private static Lidmaatschap LidmaatschapVasteDatums() =>
        new()
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Lidmaatschap.Type,
            LidmaatschapId = 1,
            Beschrijving = "Een lidmaatschap",
            Naam = "De andere vereniging",
            Van = "2002-11-15",
            Tot = "2002-11-16",
            Identificatie = "Een identificatie",
            AndereVereniging = "V0001111",
        };

    private static Lidmaatschap GewoonLidRelatief() =>
        new()
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Lidmaatschap.Type,
            LidmaatschapId = 1,
            AndereVereniging = "V0001002",
            Naam = "De andere vereniging",
            Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
            Tot = DateOnly.FromDateTime(DateTime.Today).ToString(WellknownFormats.DateOnly),
            Beschrijving = "Gewoon een lid",
            Identificatie = "L1234",
        };

    private static Lidmaatschap TijdelijkLidRelatief() =>
        new()
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "2"),
            type = JsonLdType.Lidmaatschap.Type,
            LidmaatschapId = 2,
            Naam = "Samen sterk",
            AndereVereniging = "V0001003",
            Van = DateOnly.FromDateTime(DateTime.Today.AddMonths(-5)).ToString(WellknownFormats.DateOnly),
            Tot = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)).ToString(WellknownFormats.DateOnly),
            Beschrijving = "Tijdelijk lidmaatschap",
            Identificatie = "L4321",
        };

    private static Bankrekeningnummer LidgeldBankrekening() =>
        new()
        {
            id = JsonLdType.Bankrekeningnummer.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Bankrekeningnummer.Type,
            BankrekeningnummerId = 1,
            Iban = "BE68539007547034",
            Doel = "Lidgeld",
            Titularissen = ["John Doe"],
        };
}
