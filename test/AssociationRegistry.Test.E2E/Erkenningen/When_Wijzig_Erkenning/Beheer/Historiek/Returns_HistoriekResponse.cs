namespace AssociationRegistry.Test.E2E.Erkenningen.When_Wijzig_Erkenning.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigErkenningCollection))]
public class Returns_Historiek_Met_Erkenning : End2EndTest<HistoriekResponse>
{
    private readonly WijzigErkenningContext _testContext;

    public Returns_Historiek_Met_Erkenning(WijzigErkenningContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerHistoriek(
            setup.AdminHttpClient,
            _testContext.VCode,
            headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_ErkenningWerdGewijzigd_Gebeurtenissen()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(ErkenningWerdGewijzigd)
        );

        gebeurtenisResponse.ShouldCompare(
            HistoriekGebeurtenisMapper.ErkenningWerdGewijzigd(
                _testContext.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
                _testContext.CommandRequest
            ),
            compareConfig: HistoriekComparisonConfig.Instance
        );
    }
}
