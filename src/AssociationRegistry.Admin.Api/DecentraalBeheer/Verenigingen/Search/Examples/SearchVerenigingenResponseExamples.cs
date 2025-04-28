namespace AssociationRegistry.Admin.Api.Verenigingen.Search.Examples;

using Formats;
using Hosts.Configuration.ConfigurationBindings;
using JsonLdContext;
using Vereniging;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Vereniging.Mappers;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using Vereniging = ResponseModels.Vereniging;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using Verenigingstype = ResponseModels.Verenigingstype;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class SearchVerenigingenResponseExamples : IMultipleExamplesProvider<SearchVerenigingenResponse>
{
    private readonly AppSettings _appSettings;

    public SearchVerenigingenResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public IEnumerable<SwaggerExample<SearchVerenigingenResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Zonder versie header",
            new SearchVerenigingenResponse()
            {
                Context =
                    $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/zoek-verenigingen-context.json",

                Verenigingen = new[]
                {
                    new Vereniging
                    {
                        type = JsonLdType.FeitelijkeVereniging.Type,
                        VCode = "V0001001",
                        Naam = "FWA De vrolijke BA’s",
                        KorteNaam = "DVB",
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
                        Werkingsgebieden =
                        [
                            new Werkingsgebied
                            {
                                id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                                type = JsonLdType.Werkingsgebied.Type,
                                Code = "BE25",
                                Naam = "Provincie West-Vlaanderen",
                            },
                        ],
                        Status = VerenigingStatus.Actief,
                        Startdatum = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
                                             .ToString(WellknownFormats.DateOnly),
                        Einddatum = null,
                        Doelgroep = new DoelgroepResponse
                        {
                            id = JsonLdType.Doelgroep.CreateWithIdValues("V0001001"),
                            type = JsonLdType.Doelgroep.Type,
                            Minimumleeftijd = 0,
                            Maximumleeftijd = 150,
                        },
                        Locaties = new[]
                        {
                            new Locatie
                            {
                                id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Locatie.Type,
                                Locatietype = Locatietype.Correspondentie.Waarde,
                                IsPrimair = true,
                                Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, België",
                                Naam = "",
                                Postcode = "1770",
                                Gemeente = "Liedekerke",
                            },
                        },
                        Verenigingstype = new Verenigingstype
                        {
                            Code = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
                            Naam = AssociationRegistry.Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
                        },
                        Lidmaatschappen =
                        [
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001002",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
                                              .ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today)
                                              .ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Tijdelijk lidmaatschap",
                                Identificatie = "L1234",
                            },
                        ],
                        Sleutels = new[]
                        {
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues(
                                    "V0001001", Sleutelbron.VR.Waarde),
                                type = JsonLdType.Sleutel.Type,
                                Waarde = "V0001001",
                                Bron = Sleutelbron.VR.Waarde,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                        "V0001001", Sleutelbron.VR.Waarde),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "V0001001",
                                },
                                CodeerSysteem = CodeerSysteem.VR.Waarde,
                            },
                        },
                        Links = new VerenigingLinks
                        {
                            Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0001001"),
                        },
                    },
                    new Vereniging
                    {
                        type = JsonLdType.FeitelijkeVereniging.Type,
                        VCode = "V0036651",
                        Naam = "FWA De Bron",
                        KorteNaam = string.Empty,
                        Roepnaam = "Bronneke",
                        Status = VerenigingStatus.Actief,
                        HoofdactiviteitenVerenigingsloket = new[]
                        {
                            new HoofdactiviteitVerenigingsloket
                            {
                                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("SPRT"),
                                type = JsonLdType.Hoofdactiviteit.Type,
                                Code = "SPRT",
                                Naam = "Sport",
                            },
                        },
                        Werkingsgebieden =
                        [
                            new Werkingsgebied
                            {
                                id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                                type = JsonLdType.Werkingsgebied.Type,
                                Code = "BE25",
                                Naam = "Provincie West-Vlaanderen",
                            },
                        ],
                        Verenigingstype = new Verenigingstype
                        {
                            Code = AssociationRegistry.Vereniging.Verenigingstype.VZW.Code,
                            Naam = AssociationRegistry.Vereniging.Verenigingstype.VZW.Naam,
                        },
                        Startdatum = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
                                             .ToString(WellknownFormats.DateOnly),
                        Einddatum = null,
                        Doelgroep = new DoelgroepResponse
                        {
                            id = JsonLdType.FeitelijkeVereniging.CreateWithIdValues("V00036651"),
                            type = JsonLdType.FeitelijkeVereniging.Type,
                            Minimumleeftijd = 0,
                            Maximumleeftijd = 150,
                        },
                        Locaties = new[]
                        {
                            new Locatie
                            {
                                id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Locatie.Type,
                                Locatietype = Locatietype.Activiteiten.Waarde,
                                IsPrimair = false,
                                Adresvoorstelling = "dorpstraat 91, 9000 Gent, België",
                                Naam = "Cursuszaal",
                                Postcode = "9000",
                                Gemeente = "Gent",
                            },
                        },
                        Lidmaatschappen = new[]
                        {
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001002",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
                                              .ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today)
                                              .ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Gewoon een lid",
                                Identificatie = "L1234",
                            },
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "2"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001003",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddMonths(-5))
                                              .ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
                                              .ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Tijdelijk lidmaatschap",
                                Identificatie = "L4321",
                            },
                        },
                        Sleutels = new[]
                        {
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues(
                                    "V0001002", Sleutelbron.VR.Waarde),
                                type = JsonLdType.Sleutel.Type,
                                Waarde = "V0001002",
                                Bron = Sleutelbron.VR.Waarde,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                        "V0001002", Sleutelbron.VR.Waarde),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "V0001002",
                                },
                                CodeerSysteem = CodeerSysteem.VR.Waarde,
                            },
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues(
                                    "V0001002", Sleutelbron.KBO.Waarde),
                                type = JsonLdType.Sleutel.Type,
                                Waarde = "0123456789",
                                Bron = Sleutelbron.KBO.Waarde,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                        "V0001002", Sleutelbron.KBO.Waarde),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "0123456789",
                                },
                                CodeerSysteem = CodeerSysteem.KBO.Waarde,
                            },
                        },
                        Links = new VerenigingLinks
                        {
                            Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0036651"),
                        },
                    },
                },

                Metadata = new Metadata
                {
                    Pagination = new Pagination { TotalCount = 2, Offset = 0, Limit = 50 },
                },
            });
        yield return SwaggerExample.Create(
            name: "Versie 'v2'",
            new SearchVerenigingenResponse()
            {
                Context =
                    $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/zoek-verenigingen-context.json",

                Verenigingen = new[]
                {
                    new Vereniging
                    {
                        type = JsonLdType.FeitelijkeVereniging.Type,
                        VCode = "V0001001",
                        Naam = "FWA De vrolijke BA’s",
                        KorteNaam = "DVB",
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
                        Werkingsgebieden =
                        [
                            new Werkingsgebied
                            {
                                id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                                type = JsonLdType.Werkingsgebied.Type,
                                Code = "BE25",
                                Naam = "Provincie West-Vlaanderen",
                            },
                        ],
                        Status = VerenigingStatus.Actief,
                        Startdatum = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
                                             .ToString(WellknownFormats.DateOnly),
                        Einddatum = null,
                        Doelgroep = new DoelgroepResponse
                        {
                            id = JsonLdType.Doelgroep.CreateWithIdValues("V0001001"),
                            type = JsonLdType.Doelgroep.Type,
                            Minimumleeftijd = 0,
                            Maximumleeftijd = 150,
                        },
                        Locaties = new[]
                        {
                            new Locatie
                            {
                                id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Locatie.Type,
                                Locatietype = Locatietype.Correspondentie.Waarde,
                                IsPrimair = true,
                                Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, België",
                                Naam = "",
                                Postcode = "1770",
                                Gemeente = "Liedekerke",
                            },
                        },
                        Verenigingstype = new Verenigingstype
                        {
                            Code = AssociationRegistry.Vereniging.Verenigingstype.VZER.Code,
                            Naam = AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam,
                        },
                        Verenigingssubtype = VerenigingssubtypeCode.NietBepaald.Map<Verenigingssubtype>(),
                        Lidmaatschappen = new[]
                        {
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001002",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
                                              .ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today)
                                              .ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Tijdelijk lidmaatschap",
                                Identificatie = "L1234",
                            },
                        },
                        Sleutels = new[]
                        {
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues(
                                    "V0001001", Sleutelbron.VR.Waarde),
                                type = JsonLdType.Sleutel.Type,
                                Waarde = "V0001001",
                                Bron = Sleutelbron.VR.Waarde,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                        "V0001001", Sleutelbron.VR.Waarde),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "V0001001",
                                },
                                CodeerSysteem = CodeerSysteem.VR.Waarde,
                            },
                        },
                        Links = new VerenigingLinks
                        {
                            Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0001001"),
                        },
                    },
                    new Vereniging
                    {
                        type = JsonLdType.FeitelijkeVereniging.Type,
                        VCode = "V0036651",
                        Naam = "FWA De Bron",
                        KorteNaam = string.Empty,
                        Roepnaam = "Bronneke",
                        Status = VerenigingStatus.Actief,
                        HoofdactiviteitenVerenigingsloket = new[]
                        {
                            new HoofdactiviteitVerenigingsloket
                            {
                                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("SPRT"),
                                type = JsonLdType.Hoofdactiviteit.Type,
                                Code = "SPRT",
                                Naam = "Sport",
                            },
                        },
                        Werkingsgebieden =
                        [
                            new Werkingsgebied
                            {
                                id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                                type = JsonLdType.Werkingsgebied.Type,
                                Code = "BE25",
                                Naam = "Provincie West-Vlaanderen",
                            },
                        ],
                        Verenigingstype = new Verenigingstype
                        {
                            Code = AssociationRegistry.Vereniging.Verenigingstype.VZW.Code,
                            Naam = AssociationRegistry.Vereniging.Verenigingstype.VZW.Naam,
                        },
                        Startdatum = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
                                             .ToString(WellknownFormats.DateOnly),
                        Einddatum = null,
                        Doelgroep = new DoelgroepResponse
                        {
                            id = JsonLdType.FeitelijkeVereniging.CreateWithIdValues("V00036651"),
                            type = JsonLdType.FeitelijkeVereniging.Type,
                            Minimumleeftijd = 0,
                            Maximumleeftijd = 150,
                        },
                        Locaties = new[]
                        {
                            new Locatie
                            {
                                id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Locatie.Type,
                                Locatietype = Locatietype.Activiteiten.Waarde,
                                IsPrimair = false,
                                Adresvoorstelling = "dorpstraat 91, 9000 Gent, België",
                                Naam = "Cursuszaal",
                                Postcode = "9000",
                                Gemeente = "Gent",
                            },
                        },
                        Lidmaatschappen = new[]
                        {
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001002",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
                                              .ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today)
                                              .ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Gewoon een lid",
                                Identificatie = "L1234",
                            },
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "2"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001003",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddMonths(-5))
                                              .ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
                                              .ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Tijdelijk lidmaatschap",
                                Identificatie = "L4321",
                            },
                        },
                        Sleutels = new[]
                        {
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues(
                                    "V0001002", Sleutelbron.VR.Waarde),
                                type = JsonLdType.Sleutel.Type,
                                Waarde = "V0001002",
                                Bron = Sleutelbron.VR.Waarde,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                        "V0001002", Sleutelbron.VR.Waarde),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "V0001002",
                                },
                                CodeerSysteem = CodeerSysteem.VR.Waarde,
                            },
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues(
                                    "V0001002", Sleutelbron.KBO.Waarde),
                                type = JsonLdType.Sleutel.Type,
                                Waarde = "0123456789",
                                Bron = Sleutelbron.KBO.Waarde,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                        "V0001002", Sleutelbron.KBO.Waarde),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "0123456789",
                                },
                                CodeerSysteem = CodeerSysteem.KBO.Waarde,
                            },
                        },
                        Links = new VerenigingLinks
                        {
                            Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0036651"),
                        },
                    },
                },

                Metadata = new Metadata
                {
                    Pagination = new Pagination { TotalCount = 2, Offset = 0, Limit = 50 },
                },
            });
    }
}
