namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WhenSubtypeWerdGewijzigdCollection))]
public class Returns_HistoriekResponse : End2EndTest<HistoriekResponse>
{
    private readonly WhenSubtypeWerdGewijzigdContext _testContext;

    public Returns_HistoriekResponse(WhenSubtypeWerdGewijzigdContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)).GetAwaiter().GetResult();

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
    public void With_All_Gebeurtenissen()
    {
        var relatieWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(SubverenigingRelatieWerdGewijzigd));
        relatieWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.SubverenigingRelatieWerdGewijzigd(
                                               new SubverenigingRelatieWerdGewijzigd(_testContext.VCode, _testContext.CommandRequest.AndereVereniging, _testContext.Scenario.BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam)),
                                           compareConfig: HistoriekComparisonConfig.Instance);

        var detailWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(SubverenigingDetailsWerdenGewijzigd));
        detailWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.SubverenigingDetailsWerdenGewijzigd(new SubverenigingDetailsWerdenGewijzigd(_testContext.VCode, _testContext.CommandRequest.Identificatie, _testContext.CommandRequest.Beschrijving)),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
