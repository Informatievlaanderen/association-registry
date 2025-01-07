namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Beheer.Historiek;

using Admin.Api.DecentraalBeheer.Verenigingen.Historiek.ResponseModels;
using Admin.Api.DecentraalBeheer.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Historiek : End2EndTest<WijzigLidmaatschapContext, WijzigLidmaatschapRequest, HistoriekResponse>
{
    public override Func<IApiSetup, HistoriekResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerHistoriek(TestContext.VCode);

    public Returns_Historiek(WijzigLidmaatschapContext testContext) : base(testContext)
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
        var lidmaatschapWerdToegevoegd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(LidmaatschapWerdGewijzigd));

        lidmaatschapWerdToegevoegd.ShouldCompare(HistoriekGebeurtenisMapper.LidmaatschapWerdGewijzigd(
                                                     TestContext.Request,
                                                     TestContext.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                                                     TestContext.Scenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                                                     TestContext.Scenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam),
                                                 compareConfig: HistoriekComparisonConfig.Instance);
    }
}
