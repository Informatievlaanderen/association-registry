namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Publiek.Zoeken;

using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using JsonLdContext;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Search.ResponseModels;
using Vereniging;
using Xunit;
using Vereniging = Public.Api.Verenigingen.Search.ResponseModels.Vereniging;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<WijzigLidmaatschapContext, WijzigLidmaatschapRequest, SearchVerenigingenResponse>
{
    private readonly WijzigLidmaatschapContext _testContext;
    private readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public Returns_SearchVerenigingenResponse(WijzigLidmaatschapContext testContext) : base(testContext)
    {
        _testContext = testContext;
        FeitelijkeVerenigingWerdGeregistreerd = testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd;
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
                Minimumleeftijd = FeitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = FeitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = FeitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving,
            KorteNaam = FeitelijkeVerenigingWerdGeregistreerd.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = FeitelijkeVerenigingWerdGeregistreerd.Naam,
            HoofdactiviteitenVerenigingsloket = PubliekZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = PubliekZoekResponseMapper.MapWerkingsgebieden(FeitelijkeVerenigingWerdGeregistreerd.Werkingsgebieden),
            Locaties = PubliekZoekResponseMapper.MapLocaties(FeitelijkeVerenigingWerdGeregistreerd.Locaties, _testContext.VCode),
            Lidmaatschappen = PubliekZoekResponseMapper.MapLidmaatschappen(_testContext.Request, _testContext.Scenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode),
            Relaties = [],
            Sleutels = PubliekZoekResponseMapper.MapSleutels(_testContext.VCode),
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.PublicApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}");
}
