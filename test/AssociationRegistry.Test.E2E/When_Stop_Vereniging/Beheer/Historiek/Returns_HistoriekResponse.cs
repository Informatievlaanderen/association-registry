namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(StopVerenigingCollection))]
public class Returns_HistoriekResponse : End2EndTest<HistoriekResponse>
{
    private readonly StopVerenigingContext _testContext;

    public Returns_HistoriekResponse(StopVerenigingContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(_testContext.VCode);

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(_testContext.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_VerenigingWerdGestopt_Gebeurtenis()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VerenigingWerdGestopt));

        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.VerenigingWerdGestopt(
                                                     _testContext.CommandRequest.Einddatum.Value),
                                                 compareConfig: HistoriekComparisonConfig.Instance);
    }
}
