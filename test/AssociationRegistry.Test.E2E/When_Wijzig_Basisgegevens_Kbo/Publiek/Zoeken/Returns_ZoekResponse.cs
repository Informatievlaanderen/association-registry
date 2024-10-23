namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo.Publiek.Zoeken;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Vereniging = Public.Api.Verenigingen.Search.ResponseModels.Vereniging;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<WijzigBasisgegevensKboTestContext, WijzigBasisgegevensRequest,
    SearchVerenigingenResponse>
{
    private readonly WijzigBasisgegevensKboTestContext _testContext;

    public Returns_SearchVerenigingenResponse(WijzigBasisgegevensKboTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async Task WithVerenigingMetRechtspersoonlijkheid()
        => Response.Verenigingen.Single().ShouldCompare(new Vereniging
        {
            type = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = Request.KorteBeschrijving,
            KorteNaam = _testContext.RegistratieData.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.VZW.Code,
                Naam = Verenigingstype.VZW.Naam,
            },
            Naam = _testContext.RegistratieData.Naam,
            Roepnaam = Request.Roepnaam,
            HoofdactiviteitenVerenigingsloket = PubliekZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(Request.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekZoekResponseMapper.MapWerkingsgebieden(Request.Werkingsgebieden),
            Locaties = [],
            Relaties = [],
            Sleutels = PubliekZoekResponseMapper.MapSleutels(_testContext.VCode, _testContext.RegistratieData.KboNummer),
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.PublicApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}");
}
