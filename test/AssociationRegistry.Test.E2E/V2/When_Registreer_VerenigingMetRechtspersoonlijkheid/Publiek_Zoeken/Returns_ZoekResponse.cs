namespace AssociationRegistry.Test.E2E.V2.When_Registreer_VerenigingMetRechtspersoonlijkheid.Publiek_Zoeken;

using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using HoofdactiviteitVerenigingsloket = Public.Api.Verenigingen.Search.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Public.Api.Verenigingen.Search.ResponseModels.Locatie;
using Vereniging = Public.Api.Verenigingen.Search.ResponseModels.Vereniging;
using Werkingsgebied = Public.Api.Verenigingen.Search.ResponseModels.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<RegistreerVerenigingMetRechtspersoonlijkheidTestContext, RegistreerVerenigingUitKboRequest, SearchVerenigingenResponse>
{
    private readonly RegistreerVerenigingMetRechtspersoonlijkheidTestContext _testContext;

    public Returns_SearchVerenigingenResponse(RegistreerVerenigingMetRechtspersoonlijkheidTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
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
            KorteBeschrijving = _testContext.KorteBeschrijving,
            KorteNaam = Request.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = Request.Naam,
            HoofdactiviteitenVerenigingsloket = MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = MapLocaties(Request.Locaties, _testContext.VCode),
            Relaties = [],
            Sleutels = MapSleutels(Request, _testContext.VCode),
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.PublicApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
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
        => setup => setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}");
}
