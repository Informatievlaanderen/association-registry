namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Publiek.Zoeken;

using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Framework.Comparison;
using Framework.Mappers;
using KellermanSoftware.CompareNetObjects;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Vereniging;
using Xunit;
using Vereniging = Public.Api.WebApi.Verenigingen.Search.ResponseModels.Vereniging;

[Collection(nameof(VerwijderLidmaatschapCollection))]
public class Returns_SearchVerenigingenResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerwijderLidmaatschapContext _testContext;

    public Returns_SearchVerenigingenResponse(VerwijderLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().ShouldCompare(new Vereniging()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Doelgroep.Minimumleeftijd,
                Maximumleeftijd = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Doelgroep.Maximumleeftijd,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.KorteBeschrijving,
            KorteNaam = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.KorteNaam,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = Verenigingstype.FeitelijkeVereniging.Naam,
            },
            Naam = _testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Naam,
            HoofdactiviteitenVerenigingsloket = PubliekZoekResponseMapper.MapHoofdactiviteitenVerenigingsloket(_testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.HoofdactiviteitenVerenigingsloket),
            Werkingsgebieden = [],
            Locaties = PubliekZoekResponseMapper.MapLocaties(_testContext.Scenario.BaseScenario.FeitelijkeVerenigingWerdGeristreerdMetPersoonsgegevens.Locaties, _testContext.VCode),
            Lidmaatschappen = [],
            Relaties = [],
            Sleutels = PubliekZoekResponseMapper.MapSleutels(_testContext.VCode),
            Links = new VerenigingLinks
            {
                Detail = new Uri($"{_testContext.PublicApiAppSettings.BaseUrl}/v1/verenigingen/{_testContext.VCode}"),
            },
        }, compareConfig: PubliekZoekenComparisonConfig.Instance);

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekZoeken($"vCode:{_testContext.VCode}", _testContext.CommandResult.Sequence);
}
