namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Detail.Examples;

using AssociationRegistry.Formats;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Bronnen;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
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
            name: "Feitelijke vereniging",
            new DetailVerenigingResponse
            {
                Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/detail-vereniging-context.json",
                Vereniging = new VerenigingDetail
                {
                    type = JsonLdType.FeitelijkeVereniging.Type,
                    VCode = "V0001001",
                    Verenigingstype = new VerenigingsType
                    {
                        Naam = Verenigingstype.FeitelijkeVereniging.Naam,
                        Code = Verenigingstype.FeitelijkeVereniging.Code,
                    },
                    Naam = "FWA De vrolijke BA’s",
                    KorteNaam = "DVB",
                    KorteBeschrijving = "De vereniging van de vrolijke BA's",
                    Startdatum = "2020-05-15",
                    Einddatum = "2021-12-12",
                    Doelgroep = new DoelgroepResponse
                    {
                        id = JsonLdType.Doelgroep.CreateWithIdValues("V0001001"),
                        type = JsonLdType.Doelgroep.Type,
                        Minimumleeftijd = 0,
                        Maximumleeftijd = 150,
                    },
                    Status = VerenigingStatus.Gestopt,
                    IsUitgeschrevenUitPubliekeDatastroom = false,
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloket
                        {
                            id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("CULT"),
                            type = JsonLdType.Hoofdactiviteit.Type,
                            Code = "CULT",
                            Naam = "Cultuur",
                        },
                    },
                    Werkingsgebieden = new []
                    {
                        new Werkingsgebied()
                        {
                            id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                            type = JsonLdType.Werkingsgebied.Type,
                            Code = "BE25",
                            Naam = "Provincie West-Vlaanderen",
                        }
                    },
                    Contactgegevens = new[]
                    {
                        new Contactgegeven
                        {
                            id = JsonLdType.Contactgegeven.CreateWithIdValues("V0001001", "1"),
                            type = JsonLdType.Contactgegeven.Type,
                            Contactgegeventype = "E-mail",
                            Beschrijving = "Info",
                            Waarde = "info@example.org",
                            ContactgegevenId = 1,
                            IsPrimair = false,
                            Bron = Bron.Initiator.Waarde,
                        },
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                            type = JsonLdType.Locatie.Type,
                            LocatieId = 1,
                            Locatietype = Locatietype.Correspondentie.Waarde,
                            IsPrimair = true,
                            Adresvoorstelling = "Kerkstraat 5, 1770 Liedekerke, België",
                            Naam = "Administratief centrum",
                            Adres = new Adres
                            {
                                id = JsonLdType.Adres.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Adres.Type,
                                Straatnaam = "Kerkstraat",
                                Huisnummer = "5",
                                Busnummer = "b",
                                Postcode = "1770",
                                Gemeente = "Liedekerke",
                                Land = "België",
                            },
                            AdresId = new AdresId
                            {
                                Broncode = Adresbron.AR,
                                Bronwaarde = AssociationRegistry.Vereniging.AdresId.DataVlaanderenAdresPrefix + 1,
                            },
                            VerwijstNaar =
                                new AdresVerwijzing
                                {
                                    id = JsonLdType.AdresVerwijzing.CreateWithIdValues("1"),
                                    type = JsonLdType.AdresVerwijzing.Type,
                                },
                            Bron = Bron.Initiator,
                        },
                    },
                    Vertegenwoordigers = new[]
                    {
                        new Vertegenwoordiger
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
                        },
                    },
                    Sleutels = new[]
                    {
                        new Sleutel
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
                        },
                    },
                    Relaties = Array.Empty<Relatie>(),
                    Lidmaatschappen =
                    [
                        new Lidmaatschap
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
                        },
                    ],
                    IsDubbelVan = "",
                    Bron = Bron.Initiator.Waarde,
                },
                Metadata = new Metadata
                {
                    DatumLaatsteAanpassing = "2020-05-15",
                },
            }
        );

        yield return SwaggerExample.Create(
            name: "Vereniging met rechtspersoonlijkheid",
            new DetailVerenigingResponse
            {
                Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/detail-vereniging-context.json",
                Vereniging = new VerenigingDetail
                {
                    type = JsonLdType.FeitelijkeVereniging.Type,
                    VCode = "V0001002",
                    Verenigingstype = new VerenigingsType
                    {
                        Naam = Verenigingstype.VZW.Naam,
                        Code = Verenigingstype.VZW.Code,
                    },
                    Naam = "Vissen achter het net",
                    KorteNaam = "VAN",
                    Roepnaam = "Vissen achter 't net",
                    KorteBeschrijving = "Een kleine groep vissers",
                    Startdatum = "2002-11-15",
                    Einddatum = null,
                    Doelgroep = new DoelgroepResponse
                    {
                        Minimumleeftijd = 60,
                        Maximumleeftijd = 90,
                    },
                    Status = VerenigingStatus.Actief,
                    IsUitgeschrevenUitPubliekeDatastroom = false,
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloket
                        {
                            id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("CULT"),
                            type = JsonLdType.Hoofdactiviteit.Type,
                            Code = "CULT",
                            Naam = "Cultuur",
                        },
                    },
                    Contactgegevens = new[]
                    {
                        new Contactgegeven
                        {
                            id = JsonLdType.Contactgegeven.CreateWithIdValues("V0001001", "1"),
                            type = JsonLdType.Contactgegeven.Type,
                            Contactgegeventype = "E-mail",
                            Beschrijving = "Info",
                            Waarde = "info@example.org",
                            ContactgegevenId = 1,
                            IsPrimair = false,
                            Bron = Bron.KBO.Waarde,
                        },
                        new Contactgegeven
                        {
                            id = JsonLdType.Contactgegeven.CreateWithIdValues("V0001001", "2"),
                            type = JsonLdType.Contactgegeven.Type,
                            Contactgegeventype = Contactgegeventype.SocialMedia,
                            Beschrijving = "BlubBlub",
                            Waarde = "blubblub.com/vissen",
                            ContactgegevenId = 2,
                            IsPrimair = false,
                            Bron = Bron.Initiator.Waarde,
                        },
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                            type = JsonLdType.Locatie.Type,
                            LocatieId = 1,
                            Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                            IsPrimair = true,
                            Adresvoorstelling = "Zeebank 10, 8400 Oostende, België",
                            Naam = "",
                            Adres = new Adres
                            {
                                id = JsonLdType.Adres.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Adres.Type,
                                Straatnaam = "Zeebank",
                                Huisnummer = "10",
                                Busnummer = "",
                                Postcode = "8400",
                                Gemeente = "Oostende",
                                Land = "België",
                            },
                            Bron = Bron.KBO.Waarde,
                        },
                        new Locatie
                        {
                            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "2"),
                            type = JsonLdType.Locatie.Type,
                            LocatieId = 2,
                            Locatietype = Locatietype.Activiteiten.Waarde,
                            IsPrimair = false,
                            Adresvoorstelling = "De pier 1, 8430 Westende",
                            Naam = "Vis plaats",
                            Adres = new Adres
                            {
                                id = JsonLdType.Adres.CreateWithIdValues("V0001001", "2"),
                                type = JsonLdType.Adres.Type,
                                Straatnaam = "De pier",
                                Huisnummer = "1",
                                Busnummer = "",
                                Postcode = "8430",
                                Gemeente = "Westende",
                                Land = "België",
                            },
                            AdresId = new AdresId
                            {
                                Broncode = Adresbron.AR,
                                Bronwaarde = AssociationRegistry.Vereniging.AdresId.DataVlaanderenAdresPrefix + 17,
                            },
                            VerwijstNaar = new AdresVerwijzing
                            {
                                id = JsonLdType.AdresVerwijzing.CreateWithIdValues("17"),
                                type = JsonLdType.AdresVerwijzing.Type,
                            },
                            Bron = Bron.Initiator.Waarde,
                        },
                    },
                    Vertegenwoordigers = Array.Empty<Vertegenwoordiger>(),
                    Sleutels = new[]
                    {
                        new Sleutel
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
                        },
                        new Sleutel
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
                        },
                    },
                    Relaties = Array.Empty<Relatie>(),
                    Lidmaatschappen = new []
                    {
                        new Lidmaatschap()
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
                        },
                        new Lidmaatschap()
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
                        },
                    },
                    IsDubbelVan = "",
                    Bron = Bron.KBO.Waarde,
                },
                Metadata = new Metadata
                {
                    DatumLaatsteAanpassing = "2020-05-15",
                },
            }
        );
    }
}
