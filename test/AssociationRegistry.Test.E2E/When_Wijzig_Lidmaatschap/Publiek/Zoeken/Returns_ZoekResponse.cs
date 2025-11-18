namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Publiek.Zoeken;

using Contracts.JsonLdContext;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Xunit;
using DoelgroepResponse = Public.Api.WebApi.Verenigingen.Search.ResponseModels.DoelgroepResponse;
using Vereniging = Public.Api.WebApi.Verenigingen.Search.ResponseModels.Vereniging;
using Verenigingstype = DecentraalBeheer.Vereniging.Verenigingstype;

[Collection(nameof(WijzigLidmaatschapCollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigLidmaatschapContext _testContext;

    public Returns_SearchVerenigingenResponse(WijzigLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence);

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().ShouldCompare(new Vereniging
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving,
            KorteNaam = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.Naam,
            HoofdactiviteitenVerenigingsloket = PubliekZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = [],
            Locaties = PubliekZoekResponseMapper.MapLocaties(_testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties, _testContext.VCode),
            Lidmaatschappen = PubliekZoekResponseMapper.MapLidmaatschappen(_testContext.CommandRequest, _testContext.VCode, _testContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging, _testContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId),
            Relaties = [],
            Sleutels = PubliekZoekResponseMapper.MapSleutels(_testContext.VCode),
            Links = new VerenigingLinks()
            {
                Detail = new Uri($"{_testContext.PublicApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);
}
