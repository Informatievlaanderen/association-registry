namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.Search.ResponseExamples;

using System;
using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Mappers;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
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
            ZoekResponse(VrolijkeBas(type: Verenigingstype.FeitelijkeVereniging), DeBron())
        );

        yield return SwaggerExample.Create(
            name: "Versie 'v2'",
            ZoekResponse(
                VrolijkeBas(
                    type: Verenigingstype.VZER,
                    subtype: VerenigingssubtypeCode.NietBepaald.Map<Verenigingssubtype>()
                ),
                DeBron()
            )
        );
    }

    private SearchVerenigingenResponse ZoekResponse(params Vereniging[] verenigingen) =>
        new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/zoek-verenigingen-context.json",
            Verenigingen = verenigingen,
            Facets = Facets(),
            Metadata = new Metadata
            {
                Pagination = new Pagination
                {
                    TotalCount = 2,
                    Offset = 0,
                    Limit = 50,
                },
            },
        };

    private Vereniging VrolijkeBas(Verenigingstype type, Verenigingssubtype? subtype = null) =>
        new()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            VCode = "V0001001",
            Naam = "FWA De vrolijke BA’s",
            KorteNaam = "DVB",
            KorteBeschrijving = "Een vrolijke groep van BA'ers die graag BA dingen doen.",
            Verenigingstype = new VerenigingsType { Code = type.Code, Naam = type.Naam },
            Verenigingssubtype = subtype,
            HoofdactiviteitenVerenigingsloket = new[] { Hoofdactiviteit("CULT", "Cultuur") },
            Werkingsgebieden = new[] { WerkingsgebiedWestVlaanderen() },
            Doelgroep = Doelgroep("V0001001"),
            Locaties = new[] { CorrespondentieLocatie() },
            Lidmaatschappen = new[] { GewoonLid() },
            Sleutels = new[] { VrSleutel("V0001001") },
            Relaties = Array.Empty<Relatie>(),
            Links = DetailLink("V0001001"),
        };

    private Vereniging DeBron() =>
        new()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            VCode = "V0036651",
            Naam = "FWA De Bron",
            Roepnaam = "Bronneke",
            KorteNaam = string.Empty,
            KorteBeschrijving = string.Empty,
            Verenigingstype = new VerenigingsType { Code = Verenigingstype.VZW.Code, Naam = Verenigingstype.VZW.Naam },
            HoofdactiviteitenVerenigingsloket = new[] { Hoofdactiviteit("SPRT", "Sport") },
            Doelgroep = Doelgroep("V0036651"),
            Locaties = new[] { CursuszaalGent() },
            Links = DetailLink("V0036651"),
            Lidmaatschappen = new[] { GewoonLid(), TijdelijkLid() },
            Sleutels = new[] { KboSleutel("V0036651"), VrSleutel("V0036651") },
            Relaties = Array.Empty<Relatie>(),
            Werkingsgebieden = new[] { WerkingsgebiedWestVlaanderen() },
        };

    private static HoofdactiviteitVerenigingsloket Hoofdactiviteit(string code, string naam) =>
        new()
        {
            id = JsonLdType.Hoofdactiviteit.CreateWithIdValues(code),
            type = JsonLdType.Hoofdactiviteit.Type,
            Code = code,
            Naam = naam,
        };

    private static Werkingsgebied WerkingsgebiedWestVlaanderen() =>
        new()
        {
            id = JsonLdType.Werkingsgebied.CreateWithIdValues("BE25"),
            type = JsonLdType.Werkingsgebied.Type,
            Code = "BE25",
            Naam = "Provincie West-Vlaanderen",
        };

    private static DoelgroepResponse Doelgroep(string vCode) =>
        new()
        {
            id = JsonLdType.Doelgroep.CreateWithIdValues(vCode),
            type = JsonLdType.Doelgroep.Type,
            Minimumleeftijd = 0,
            Maximumleeftijd = 150,
        };

    private static Locatie CorrespondentieLocatie() =>
        new()
        {
            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Locatie.Type,
            Locatietype = Locatietype.Correspondentie.Waarde,
            IsPrimair = true,
            Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, België",
            Naam = "",
            Postcode = "1770",
            Gemeente = "Liedekerke",
        };

    private static Locatie CursuszaalGent() =>
        new()
        {
            id = JsonLdType.Locatie.CreateWithIdValues("V0036651", "1"),
            type = JsonLdType.Locatie.Type,
            Locatietype = Locatietype.Activiteiten.Waarde,
            IsPrimair = false,
            Adresvoorstelling = "dorpstraat 91, 9000 Gent, België",
            Naam = "Cursuszaal",
            Postcode = "9000",
            Gemeente = "Gent",
        };

    private static Lidmaatschap GewoonLid() =>
        new()
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Lidmaatschap.Type,
            AndereVereniging = "V0001002",
            Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
            Tot = DateOnly.FromDateTime(DateTime.Today).ToString(WellknownFormats.DateOnly),
            Beschrijving = "Gewoon een lid",
            Identificatie = "L1234",
        };

    private static Lidmaatschap TijdelijkLid() =>
        new()
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "2"),
            type = JsonLdType.Lidmaatschap.Type,
            AndereVereniging = "V0001003",
            Van = DateOnly.FromDateTime(DateTime.Today.AddMonths(-5)).ToString(WellknownFormats.DateOnly),
            Tot = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)).ToString(WellknownFormats.DateOnly),
            Beschrijving = "Tijdelijk lidmaatschap",
            Identificatie = "L4321",
        };

    private static Sleutel VrSleutel(string vCode) =>
        new()
        {
            id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR),
            type = JsonLdType.Sleutel.Type,
            Bron = Sleutelbron.VR,
            Waarde = vCode,
            CodeerSysteem = CodeerSysteem.VR,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR),
                type = JsonLdType.GestructureerdeSleutel.Type,
                Nummer = vCode,
            },
        };

    private static Sleutel KboSleutel(string vCode) =>
        new()
        {
            id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
            type = JsonLdType.Sleutel.Type,
            Waarde = "0123456789",
            Bron = Sleutelbron.KBO.Waarde,
            CodeerSysteem = CodeerSysteem.KBO,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
                type = JsonLdType.GestructureerdeSleutel.Type,
                Nummer = "0123456789",
            },
        };

    private VerenigingLinks DetailLink(string vCode) =>
        new() { Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/{vCode}") };

    private Facets Facets() =>
        new() { HoofdactiviteitenVerenigingsloket = new[] { FacetItem("CULT"), FacetItem("SPRT") } };

    private HoofdactiviteitVerenigingsloketFacetItem FacetItem(string code) =>
        new()
        {
            Code = code,
            Aantal = 1,
            Query = $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:{code})",
        };
}
