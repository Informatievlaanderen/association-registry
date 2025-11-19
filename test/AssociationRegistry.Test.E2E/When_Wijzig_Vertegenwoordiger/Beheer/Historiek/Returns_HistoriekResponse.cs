namespace AssociationRegistry.Test.E2E.When_Wijzig_Vertegenwoordiger.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigVertegenwoordigerCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<HistoriekResponse>
{
    private readonly WijzigVertegenwoordigerContext _testContext;

    public Returns_Detail_With_Lidmaatschap(WijzigVertegenwoordigerContext testContext) : base(testContext.ApiSetup)
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
        var nextVertegenwoordigerId = _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                                           .Vertegenwoordigers.Max(x => x.VertegenwoordigerId) + 1;
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigd));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdGewijzigd(_testContext.Scenario.VertegenwoordigerWerdToegevoegd, _testContext.CommandRequest),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
