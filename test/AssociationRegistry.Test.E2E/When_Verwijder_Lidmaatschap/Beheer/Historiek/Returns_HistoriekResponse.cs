namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Framework.Comparison;
using Framework.Mappers;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VerwijderLidmaatschapCollection))]
public class Returns_Historiek : End2EndTest<HistoriekResponse>
{
    private readonly VerwijderLidmaatschapContext _testContext;
    public Returns_Historiek(VerwijderLidmaatschapContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

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
        var lidmaatschapWerdVerwijderd = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(LidmaatschapWerdVerwijderd));

        lidmaatschapWerdVerwijderd.ShouldCompare(
            HistoriekGebeurtenisMapper.LidmaatschapWerdVerwijderd(_testContext.Scenario.LidmaatschapWerdToegevoegd),
                                                 compareConfig: HistoriekComparisonConfig.Instance);
    }

    public override HistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerHistoriek(_testContext.VCode);
}
