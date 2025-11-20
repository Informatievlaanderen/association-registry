namespace AssociationRegistry.Test.E2E.When_Wijzig_Vertegenwoordiger_Voor_Rechtspersoon.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigVertegenwoordigerVoorRechtspersoonCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<HistoriekResponse>
{
    private readonly WijzigVertegenwoordigerContextVoorRechtspersoon _testContext;

    public Returns_Detail_With_Lidmaatschap(WijzigVertegenwoordigerContextVoorRechtspersoon testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_LidmaatschapWerdToegevoegd_Gebeurtenissen()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigd));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdGewijzigd(_testContext.Scenario.VertegenwoordigerWerdOvergenomenUitKBO, _testContext.CommandRequest),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
