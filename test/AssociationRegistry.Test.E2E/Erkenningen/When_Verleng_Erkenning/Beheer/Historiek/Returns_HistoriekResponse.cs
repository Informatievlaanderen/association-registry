namespace AssociationRegistry.Test.E2E.Erkenningen.When_Verleng_Erkenning.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using Grpc.Core;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VerlengErkenningCollection))]
public class Returns_Historiek_Met_Erkenning : End2EndTest<HistoriekResponse>
{
    private readonly VerlengErkenningContext _testContext;

    public Returns_Historiek_Met_Erkenning(VerlengErkenningContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient,
                                                       _testContext.VCode,
                                                       headers: new RequestParameters().WithExpectedSequence(
                                                           _testContext.CommandResult.Sequence));

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_ErkenningWerdVerlengd_Gebeurtenissen()
    {
        var gebeurtenisResponse =
            Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(ErkenningWerdVerlengd));

        var expectedEvent = new ErkenningWerdVerlengd(
            ErkenningId: _testContext.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
            Einddatum: _testContext.CommandRequest.Einddatum,
            Hernieuwingsdatum: _testContext.CommandRequest.Hernieuwingsdatum.Value,
            Status: _testContext.Scenario.ErkenningWerdGeregistreerd.Status
        );

        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.ErkenningWerdVerlengd(expectedEvent),
                                          compareConfig: HistoriekComparisonConfig.Instance);
    }
}
