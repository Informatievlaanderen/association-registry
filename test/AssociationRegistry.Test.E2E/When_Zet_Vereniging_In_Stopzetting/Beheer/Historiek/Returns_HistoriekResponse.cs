namespace AssociationRegistry.Test.E2E.When_Zet_Vereniging_In_Stopzetting.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(ZetVerenigingInStopzettingCollection))]
public class Returns_HistoriekResponse : End2EndTest<HistoriekResponse>
{
    private readonly ZetVerenigingInStopzettingContext _testInStopzettingContext;

    public Returns_HistoriekResponse(ZetVerenigingInStopzettingContext testInStopzettingContext)
        : base(testInStopzettingContext.ApiSetup)
    {
        _testInStopzettingContext = testInStopzettingContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerHistoriek(
            setup.AdminHttpClient,
            _testInStopzettingContext.CommandResult.VCode,
            headers: new RequestParameters().WithExpectedSequence(_testInStopzettingContext.CommandResult.Sequence)
        );

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(_testInStopzettingContext.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_VerenigingWerdInStopzettingGeplaatst_Gebeurtenis()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(VerenigingWerdInStopzettingGeplaatst)
        );

        gebeurtenisResponse.ShouldCompare(
            HistoriekGebeurtenisMapper.VerenigingWerdInStopzettingGeplaatst(),
            compareConfig: HistoriekComparisonConfig.Instance
        );
    }
}
