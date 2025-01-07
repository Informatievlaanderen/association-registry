namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging.Beheer.Zoeken;

using Admin.Api.DecentraalBeheer.Verenigingen.Common;
using Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Api.DecentraalBeheer.Verenigingen.Search.ResponseModels;
using Formats;
using JsonLdContext;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.TestClasses;
using Vereniging;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;
using HoofdactiviteitVerenigingsloket = Admin.Api.DecentraalBeheer.Verenigingen.Search.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Api.DecentraalBeheer.Verenigingen.Search.ResponseModels.Locatie;
using Vereniging = Admin.Api.DecentraalBeheer.Verenigingen.Search.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Werkingsgebied = Admin.Api.DecentraalBeheer.Verenigingen.Search.ResponseModels.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, SearchVerenigingenResponse>
{
    private readonly RegistreerFeitelijkeVerenigingTestContext _testContext;

    public Returns_SearchVerenigingenResponse(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async Task WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().ShouldCompare(new Vereniging
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _testContext.VCode,
            KorteNaam = Request.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = Request.Naam,
            Startdatum = Instant.FromDateTimeOffset(DateTimeOffset.UtcNow).FormatAsBelgianDate(),
            Einddatum = null,
            Status = VerenigingStatus.Actief,
            HoofdactiviteitenVerenigingsloket = MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = MapLocaties(Request.Locaties, _testContext.VCode),
            Sleutels = MapSleutels(Request, _testContext.VCode),
            Lidmaatschappen = [],
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.AdminApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

    private static Sleutel[] MapSleutels(RegistreerFeitelijkeVerenigingRequest request, string vCode)
        =>
        [
            new Sleutel
            {
                Bron = Sleutelbron.VR.Waarde,
                id = JsonLdType.Sleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                type = JsonLdType.Sleutel.Type,
                Waarde = vCode,
                CodeerSysteem = CodeerSysteem.VR,
                GestructureerdeIdentificator = new GestructureerdeIdentificator
                {
                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(vCode, Sleutelbron.VR.Waarde),
                    type = JsonLdType.GestructureerdeSleutel.Type,
                    Nummer = vCode,
                },
            },
        ];

    private static Locatie[] MapLocaties(ToeTeVoegenLocatie[] toeTeVoegenLocaties, string vCode)
    {
        return toeTeVoegenLocaties.Select((x, i) => new Locatie
        {
            id = JsonLdType.Locatie.CreateWithIdValues(
                vCode, $"{i + 1}"),
            type = JsonLdType.Locatie.Type,
            Locatietype = x.Locatietype,
            Naam = x.Naam,
            IsPrimair = x.IsPrimair,
        }).ToArray();
    }

    private static HoofdactiviteitVerenigingsloket[] MapHoofdactiviteitenVerenigingsloket(
        string[] hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x =>
        {
            var hoofdactiviteitVerenigingsloket = AssociationRegistry.Vereniging.HoofdactiviteitVerenigingsloket.Create(x);

            return new HoofdactiviteitVerenigingsloket
            {
                Code = hoofdactiviteitVerenigingsloket.Code,
                Naam = hoofdactiviteitVerenigingsloket.Naam,
                id = JsonLdType.Hoofdactiviteit.CreateWithIdValues(hoofdactiviteitVerenigingsloket.Code),
                type = JsonLdType.Hoofdactiviteit.Type,
            };
        }).ToArray();
    }

    private static Werkingsgebied[] MapWerkingsgebieden(
        string[] werkingsgebieden)
    {
        return werkingsgebieden.Select(x =>
        {
            var werkingsgebied = AssociationRegistry.Vereniging.Werkingsgebied.Create(x);

            return new Werkingsgebied
            {
                Code = werkingsgebied.Code,
                Naam = werkingsgebied.Naam,
                id = JsonLdType.Werkingsgebied.CreateWithIdValues(werkingsgebied.Code),
                type = JsonLdType.Werkingsgebied.Type,
            };
        }).ToArray();
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.VCode}");
}
