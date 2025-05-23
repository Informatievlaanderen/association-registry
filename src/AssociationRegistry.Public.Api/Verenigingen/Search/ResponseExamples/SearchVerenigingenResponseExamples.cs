namespace AssociationRegistry.Public.Api.Verenigingen.Search.ResponseExamples;

using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Vereniging;
using Swashbuckle.AspNetCore.Filters;
using System;
using Vereniging.Mappers;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using Vereniging = ResponseModels.Vereniging;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
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
                Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/zoek-verenigingen-context.json",
                Verenigingen = new[]
                {
                    new Vereniging
                    {
                        type = JsonLdType.FeitelijkeVereniging.Type, VCode = "V0001001",
                        Naam = "FWA De vrolijke BA’s",
                        KorteNaam = "DVB",
                        KorteBeschrijving = "Een vrolijke groep van BA'ers die graag BA dingen doen.",
                        Verenigingstype = new VerenigingsType
                        {
                            Code = Verenigingstype.FeitelijkeVereniging.Code,
                            Naam = Verenigingstype.FeitelijkeVereniging.Naam,
                        },
                        HoofdactiviteitenVerenigingsloket = new[]
                        {
                            new HoofdactiviteitVerenigingsloket
                            {
                                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("CULT"),
                                type = JsonLdType.Hoofdactiviteit.Type,
                                Code = "CULT", Naam = "Cultuur",
                            },
                        },
                        Werkingsgebieden = new[]
                        {
                            new Werkingsgebied
                            {
                                id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                                type = JsonLdType.Werkingsgebied.Type,
                                Code = "BE25", Naam = "Provincie West-Vlaanderen",
                            },
                        },
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
                        Lidmaatschappen = new[]
                        {
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001002",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today).ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Gewoon een lid",
                                Identificatie = "L1234",
                            },
                        },
                        Sleutels =
                            new[]
                            {
                                new Sleutel
                                {
                                    id = JsonLdType.Sleutel.CreateWithIdValues("V0001001", Sleutelbron.VR),
                                    type = JsonLdType.Sleutel.Type,
                                    Bron = Sleutelbron.VR,
                                    Waarde = "V0001001",
                                    CodeerSysteem = CodeerSysteem.VR,
                                    GestructureerdeIdentificator = new GestructureerdeIdentificator
                                    {
                                        id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0001001", Sleutelbron.VR),
                                        type = JsonLdType.GestructureerdeSleutel.Type,
                                        Nummer = "V0001001",
                                    },
                                },
                            },
                        Relaties = Array.Empty<Relatie>(),
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
                        Roepnaam = "Bronneke",
                        KorteNaam = string.Empty,
                        KorteBeschrijving = string.Empty,
                        Verenigingstype = new VerenigingsType
                        {
                            Code = Verenigingstype.VZW.Code,
                            Naam = Verenigingstype.VZW.Naam,
                        },
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
                        Doelgroep = new DoelgroepResponse
                        {
                            id = JsonLdType.Doelgroep.CreateWithIdValues("V0036651"),
                            type = JsonLdType.Doelgroep.Type, Minimumleeftijd = 0,
                            Maximumleeftijd = 150,
                        },
                        Locaties = new[]
                        {
                            new Locatie
                            {
                                id = JsonLdType.Locatie.CreateWithIdValues("V0036651", "1"),
                                type = JsonLdType.Locatie.Type,
                                Locatietype = Locatietype.Activiteiten.Waarde,
                                IsPrimair = false,
                                Adresvoorstelling = "dorpstraat 91, 9000 Gent, België",
                                Naam = "Cursuszaal",
                                Postcode = "9000",
                                Gemeente = "Gent",
                            },
                        },
                        Links = new VerenigingLinks
                        {
                            Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0036651"),
                        },
                        Lidmaatschappen = new[]
                        {
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001002",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today).ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Gewoon een lid",
                                Identificatie = "L1234",
                            },
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "2"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001003",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddMonths(-5)).ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)).ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Tijdelijk lidmaatschap",
                                Identificatie = "L4321",
                            },
                        },
                        Sleutels = new[]
                        {
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues("V0036651", Sleutelbron.KBO.Waarde),
                                type = JsonLdType.Sleutel.Type,
                                Waarde = "0123456789",
                                Bron = Sleutelbron.KBO.Waarde,
                                CodeerSysteem = CodeerSysteem.KBO,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0036651", Sleutelbron.KBO.Waarde),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "0123456789",
                                },
                            },
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues("V0036651", Sleutelbron.VR),
                                type = JsonLdType.Sleutel.Type,
                                Bron = Sleutelbron.VR,
                                Waarde = "V0036651",
                                CodeerSysteem = CodeerSysteem.VR,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0036651", Sleutelbron.VR),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "V0036651",
                                },
                            },
                        },
                        Relaties = Array.Empty<Relatie>(),
                        Werkingsgebieden = new[]
                        {
                            new Werkingsgebied
                            {
                                id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                                type = JsonLdType.Werkingsgebied.Type,
                                Code = "BE25", Naam = "Provincie West-Vlaanderen",
                            },
                        },
                    },
                },
                Facets = new Facets
                {
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloketFacetItem
                        {
                            Code = "CULT",
                            Aantal = 1,
                            Query = $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:CULT)",
                        },
                        new HoofdactiviteitVerenigingsloketFacetItem
                        {
                            Code = "SPRT",
                            Aantal = 1,
                            Query = $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:SPRT)",
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
                Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/zoek-verenigingen-context.json",
                Verenigingen = new[]
                {
                    new Vereniging
                    {
                        type = JsonLdType.FeitelijkeVereniging.Type, VCode = "V0001001",
                        Naam = "FWA De vrolijke BA’s",
                        KorteNaam = "DVB",
                        KorteBeschrijving = "Een vrolijke groep van BA'ers die graag BA dingen doen.",
                        Verenigingstype = new VerenigingsType
                        {
                            Code = Verenigingstype.VZER.Code,
                            Naam = Verenigingstype.VZER.Naam,
                        },
                        Verenigingssubtype = VerenigingssubtypeCode.NietBepaald.Map<Verenigingssubtype>(),
                        HoofdactiviteitenVerenigingsloket = new[]
                        {
                            new HoofdactiviteitVerenigingsloket
                            {
                                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("CULT"),
                                type = JsonLdType.Hoofdactiviteit.Type,
                                Code = "CULT", Naam = "Cultuur",
                            },
                        },
                        Werkingsgebieden = new[]
                        {
                            new Werkingsgebied
                            {
                                id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                                type = JsonLdType.Werkingsgebied.Type,
                                Code = "BE25", Naam = "Provincie West-Vlaanderen",
                            },
                        },
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
                        Lidmaatschappen = new[]
                        {
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001002",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today).ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Gewoon een lid",
                                Identificatie = "L1234",
                            },
                        },
                        Sleutels =
                            new[]
                            {
                                new Sleutel
                                {
                                    id = JsonLdType.Sleutel.CreateWithIdValues("V0001001", Sleutelbron.VR),
                                    type = JsonLdType.Sleutel.Type,
                                    Bron = Sleutelbron.VR,
                                    Waarde = "V0001001",
                                    CodeerSysteem = CodeerSysteem.VR,
                                    GestructureerdeIdentificator = new GestructureerdeIdentificator
                                    {
                                        id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0001001", Sleutelbron.VR),
                                        type = JsonLdType.GestructureerdeSleutel.Type,
                                        Nummer = "V0001001",
                                    },
                                },
                            },
                        Relaties = Array.Empty<Relatie>(),
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
                        Roepnaam = "Bronneke",
                        KorteNaam = string.Empty,
                        KorteBeschrijving = string.Empty,
                        Verenigingstype = new VerenigingsType
                        {
                            Code = Verenigingstype.VZW.Code,
                            Naam = Verenigingstype.VZW.Naam,
                        },
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
                        Doelgroep = new DoelgroepResponse
                        {
                            id = JsonLdType.Doelgroep.CreateWithIdValues("V0036651"),
                            type = JsonLdType.Doelgroep.Type, Minimumleeftijd = 0,
                            Maximumleeftijd = 150,
                        },
                        Locaties = new[]
                        {
                            new Locatie
                            {
                                id = JsonLdType.Locatie.CreateWithIdValues("V0036651", "1"),
                                type = JsonLdType.Locatie.Type,
                                Locatietype = Locatietype.Activiteiten.Waarde,
                                IsPrimair = false,
                                Adresvoorstelling = "dorpstraat 91, 9000 Gent, België",
                                Naam = "Cursuszaal",
                                Postcode = "9000",
                                Gemeente = "Gent",
                            },
                        },
                        Links = new VerenigingLinks
                        {
                            Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0036651"),
                        },
                        Lidmaatschappen = new[]
                        {
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001002",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today).ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Gewoon een lid",
                                Identificatie = "L1234",
                            },
                            new Lidmaatschap()
                            {
                                id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "2"),
                                type = JsonLdType.Lidmaatschap.Type,
                                AndereVereniging = "V0001003",
                                Van = DateOnly.FromDateTime(DateTime.Today.AddMonths(-5)).ToString(WellknownFormats.DateOnly),
                                Tot = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)).ToString(WellknownFormats.DateOnly),
                                Beschrijving = "Tijdelijk lidmaatschap",
                                Identificatie = "L4321",
                            },
                        },
                        Sleutels = new[]
                        {
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues("V0036651", Sleutelbron.KBO.Waarde),
                                type = JsonLdType.Sleutel.Type,
                                Waarde = "0123456789",
                                Bron = Sleutelbron.KBO.Waarde,
                                CodeerSysteem = CodeerSysteem.KBO,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0036651", Sleutelbron.KBO.Waarde),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "0123456789",
                                },
                            },
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues("V0036651", Sleutelbron.VR),
                                type = JsonLdType.Sleutel.Type,
                                Bron = Sleutelbron.VR,
                                Waarde = "V0036651",
                                CodeerSysteem = CodeerSysteem.VR,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0036651", Sleutelbron.VR),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "V0036651",
                                },
                            },
                        },
                        Relaties = Array.Empty<Relatie>(),
                        Werkingsgebieden = new[]
                        {
                            new Werkingsgebied
                            {
                                id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
                                type = JsonLdType.Werkingsgebied.Type,
                                Code = "BE25", Naam = "Provincie West-Vlaanderen",
                            },
                        },
                    },
                },
                Facets = new Facets
                {
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloketFacetItem
                        {
                            Code = "CULT",
                            Aantal = 1,
                            Query = $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:CULT)",
                        },
                        new HoofdactiviteitVerenigingsloketFacetItem
                        {
                            Code = "SPRT",
                            Aantal = 1,
                            Query = $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:SPRT)",
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
