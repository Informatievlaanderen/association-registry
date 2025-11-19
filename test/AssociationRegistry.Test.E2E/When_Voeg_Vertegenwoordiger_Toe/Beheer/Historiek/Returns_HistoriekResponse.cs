namespace AssociationRegistry.Test.E2E.When_Voeg_Vertegenwoordiger_Toe.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using When_Voeg_Lidmaatschap_Toe;
using Xunit;

[Collection(nameof(VoegVertegenwoordigerToeCollection))]
public class Returns_Detail_With_Lidmaatschap : End2EndTest<HistoriekResponse>
{
    private readonly VoegVertegenwoordigerToeContext _testContext;

    public Returns_Detail_With_Lidmaatschap(VoegVertegenwoordigerToeContext testContext) : base(testContext.ApiSetup)
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
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdToegevoegd));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdToegevoegd(nextVertegenwoordigerId,_testContext.CommandRequest),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
