namespace AssociationRegistry.Test.E2E.Erkenningen.When_Hef_Schorsings_Erkenning_Op.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using DecentraalBeheer.Vereniging.Erkenningen;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(HefSchorsingErkenningOpCollection))]
public class Returns_Historiek_Met_Erkenning : End2EndTest<HistoriekResponse>
{
    private readonly HefSchorsingErkenningOpContext _testContext;

    public Returns_Historiek_Met_Erkenning(HefSchorsingErkenningOpContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerHistoriek(
            setup.AdminHttpClient,
            _testContext.VCode,
            headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_ErkenningWerdGerigistreerd_Gebeurtenissen()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(SchorsingVanErkenningWerdOpgeheven)
        );

        var status = ErkenningStatus.Bepaal(
            ErkenningsPeriode.Create(
                _testContext.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                _testContext.Scenario.ErkenningWerdGeregistreerd.Einddatum
            ),
            DateOnly.FromDateTime(DateTime.Now)
        );

        var expectedEvent = new SchorsingVanErkenningWerdOpgeheven(
            ErkenningId: _testContext.Scenario.ErkenningWerdGeregistreerd.ErkenningId,
            status
        );

        gebeurtenisResponse.ShouldCompare(
            HistoriekGebeurtenisMapper.SchorsingVanErkenningWerdOpgeheven(expectedEvent),
            compareConfig: HistoriekComparisonConfig.Instance
        );
    }
}
