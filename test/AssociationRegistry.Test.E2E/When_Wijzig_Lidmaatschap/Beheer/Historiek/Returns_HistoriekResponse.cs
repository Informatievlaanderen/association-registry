namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap.Beheer.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Scenarios.Givens.FeitelijkeVereniging;
using Xunit;

[Collection(nameof(WijzigLidmaatschapCollection))]
public class Returns_Historiek: End2EndTest<HistoriekResponse>
{
    private readonly WijzigLidmaatschapContext _testContext;
    private readonly LidmaatschapWerdToegevoegdScenario _castedScenario;

    public Returns_Historiek(WijzigLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _castedScenario = (LidmaatschapWerdToegevoegdScenario)testContext.Scenario;
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
        var lidmaatschapWerdToegevoegd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(LidmaatschapWerdGewijzigd));

        lidmaatschapWerdToegevoegd.ShouldCompare(HistoriekGebeurtenisMapper.LidmaatschapWerdGewijzigd(
                                                     _testContext.CommandRequest,
                                                     _castedScenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                                                     _castedScenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                                                     _castedScenario.BaseScenario.AndereFeitelijkeVerenigingWerdGeregistreerd.Naam),
                                                 compareConfig: HistoriekComparisonConfig.Instance);
    }
}
