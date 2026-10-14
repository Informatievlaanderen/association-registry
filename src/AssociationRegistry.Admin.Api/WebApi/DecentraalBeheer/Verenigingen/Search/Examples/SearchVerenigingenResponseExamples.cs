namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.Examples;

using AssociationRegistry.Formats;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Mappers;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
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
            ZoekResponse(
                VrolijkeBas(type: DecentraalBeheer.Vereniging.Verenigingstype.FeitelijkeVereniging, subtype: null),
                DeBron()
            )
        );

        yield return SwaggerExample.Create(
            name: "Versie 'v2'",
            ZoekResponse(
                VrolijkeBas(
                    type: DecentraalBeheer.Vereniging.Verenigingstype.VZER,
                    subtype: VerenigingssubtypeCode.NietBepaald.Map<Verenigingssubtype>()
                ),
                DeBron()
            )
        );
    }

    private SearchVerenigingenResponse ZoekResponse(params Vereniging[] verenigingen) =>
        new()
        {
            Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/zoek-verenigingen-context.json",
            Verenigingen = verenigingen,
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

    private Vereniging VrolijkeBas(DecentraalBeheer.Vereniging.Verenigingstype type, Verenigingssubtype? subtype) =>
        new()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            VCode = "V0001001",
            Naam = "FWA De vrolijke BA’s",
            KorteNaam = "DVB",
            HoofdactiviteitenVerenigingsloket = new[] { Hoofdactiviteit("CULT", "Cultuur") },
            Werkingsgebieden = new[] { WerkingsgebiedWestVlaanderen() },
            Status = VerenigingStatus.Actief,
            Startdatum = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
            Einddatum = null,
            Doelgroep = DoelgroepV0001001(),
            Locaties = new[] { CorrespondentieLocatie() },
            Verenigingstype = new Verenigingstype { Code = type.Code, Naam = type.Naam },
            Verenigingssubtype = subtype,
            Lidmaatschappen = new[] { LidmaatschapTijdelijk() },
            Sleutels = new[] { VrSleutel("V0001001") },
            Links = DetailLink("V0001001"),
        };

    private Vereniging DeBron() =>
        new()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            VCode = "V0036651",
            Naam = "FWA De Bron",
            KorteNaam = string.Empty,
            Roepnaam = "Bronneke",
            Status = VerenigingStatus.Actief,
            HoofdactiviteitenVerenigingsloket = new[] { Hoofdactiviteit("SPRT", "Sport") },
            Werkingsgebieden = new[] { WerkingsgebiedWestVlaanderen() },
            Verenigingstype = new Verenigingstype
            {
                Code = DecentraalBeheer.Vereniging.Verenigingstype.VZW.Code,
                Naam = DecentraalBeheer.Vereniging.Verenigingstype.VZW.Naam,
            },
            Startdatum = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
            Einddatum = null,
            Doelgroep = new DoelgroepResponse
            {
                id = JsonLdType.FeitelijkeVereniging.CreateWithIdValues("V00036651"),
                type = JsonLdType.FeitelijkeVereniging.Type,
                Minimumleeftijd = 0,
                Maximumleeftijd = 150,
            },
            Locaties = new[] { CursuszaalGent() },
            Lidmaatschappen = new[] { GewoonLid(), TijdelijkLid() },
            Sleutels = new[] { VrSleutel("V0001002"), KboSleutel("V0001002") },
            Links = DetailLink("V0036651"),
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

    private static DoelgroepResponse DoelgroepV0001001() =>
        new()
        {
            id = JsonLdType.Doelgroep.CreateWithIdValues("V0001001"),
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
            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Locatie.Type,
            Locatietype = Locatietype.Activiteiten.Waarde,
            IsPrimair = false,
            Adresvoorstelling = "dorpstraat 91, 9000 Gent, België",
            Naam = "Cursuszaal",
            Postcode = "9000",
            Gemeente = "Gent",
        };

    private static Lidmaatschap LidmaatschapTijdelijk() =>
        new()
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Lidmaatschap.Type,
            AndereVereniging = "V0001002",
            Van = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)).ToString(WellknownFormats.DateOnly),
            Tot = DateOnly.FromDateTime(DateTime.Today).ToString(WellknownFormats.DateOnly),
            Beschrijving = "Tijdelijk lidmaatschap",
            Identificatie = "L1234",
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
            id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
            type = JsonLdType.Sleutel.Type,
            Waarde = vCode,
            Bron = Sleutelbron.VR.Waarde,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                type = JsonLdType.GestructureerdeSleutel.Type,
                Nummer = vCode,
            },
            CodeerSysteem = CodeerSysteem.VR.Waarde,
        };

    private static Sleutel KboSleutel(string vCode) =>
        new()
        {
            id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
            type = JsonLdType.Sleutel.Type,
            Waarde = "0123456789",
            Bron = Sleutelbron.KBO.Waarde,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.KBO.Waarde),
                type = JsonLdType.GestructureerdeSleutel.Type,
                Nummer = "0123456789",
            },
            CodeerSysteem = CodeerSysteem.KBO.Waarde,
        };

    private VerenigingLinks DetailLink(string vCode) =>
        new() { Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/{vCode}") };
}
