namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe.Beheer.Historiek;

using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VoegLidmaatschapToeCollection))]
public class Returns_Historiek : End2EndTest<HistoriekResponse>
{
    private readonly VoegLidmaatschapToeContext _testContext;

    public Returns_Historiek(VoegLidmaatschapToeContext testContext)
        : base(testContext.ApiSetup)
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
    public void With_LidmaatschapWerdToegevoegd_Gebeurtenissen()
    {
        var lidmaatschapWerdToegevoegd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(LidmaatschapWerdToegevoegd));
        lidmaatschapWerdToegevoegd.ShouldCompare(HistoriekGebeurtenisMapper.LidmaatschapWerdToegevoegd(_testContext.CommandRequest, _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam), compareConfig: HistoriekComparisonConfig.Instance);
    }
}
