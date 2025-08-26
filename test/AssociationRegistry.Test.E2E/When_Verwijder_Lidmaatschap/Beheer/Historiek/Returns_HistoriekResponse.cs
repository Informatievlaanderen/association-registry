namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
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

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));
}
