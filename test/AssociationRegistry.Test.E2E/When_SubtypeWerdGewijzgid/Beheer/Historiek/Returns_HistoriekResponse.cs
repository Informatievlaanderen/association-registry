namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid.Beheer.Historiek;

using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Subtype.RequestModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : End2EndTest<WhenSubtypeWerdGewijzigdContext, WijzigSubtypeRequest, HistoriekResponse>
{
    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.VCode);

    public Returns_Historiek(WhenSubtypeWerdGewijzigdContext testContext)
    {
    }

    [Fact]
    public void With_VCode()
    {
        Response.VCode.ShouldCompare(TestContext.VCode);
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
                                               new SubverenigingRelatieWerdGewijzigd(TestContext.VCode, TestContext.Request.AndereVereniging, TestContext.Scenario.BaseScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam)),
                                           compareConfig: HistoriekComparisonConfig.Instance);

        var detailWerdGewijzigd =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(SubverenigingDetailsWerdenGewijzigd));
        detailWerdGewijzigd.ShouldCompare(HistoriekGebeurtenisMapper.SubverenigingDetailsWerdenGewijzigd(new SubverenigingDetailsWerdenGewijzigd(TestContext.VCode, TestContext.Request.Identificatie, TestContext.Request.Beschrijving)),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
