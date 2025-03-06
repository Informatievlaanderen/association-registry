namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(WellKnownCollections.VoegLidmaatschapToe)]
public class Returns_Historiek : End2EndTest<VoegLidmaatschapToeContext, VoegLidmaatschapToeRequest, HistoriekResponse>
{
    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.VCode);

    public Returns_Historiek(VoegLidmaatschapToeContext testContext) : base(testContext)
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
    public void With_LidmaatschapWerdToegevoegd_Gebeurtenissen()
    {
        var lidmaatschapWerdToegevoegd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(LidmaatschapWerdToegevoegd));
        lidmaatschapWerdToegevoegd.ShouldCompare(HistoriekGebeurtenisMapper.LidmaatschapWerdToegevoegd(TestContext.Request, TestContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
