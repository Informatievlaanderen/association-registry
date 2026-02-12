namespace AssociationRegistry.Test.E2E.When_Voeg_Bankrekeningnummer_Toe.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using When_Voeg_Contactgegeven_Toe;
using Xunit;

[Collection(nameof(VoegBankrekeningnummerToeCollection))]
public class Returns_Historiek_Met_Bankrekeningnummer : End2EndTest<HistoriekResponse>
{
    private readonly VoegBankrekeningnummerToeContext _testContext;

    public Returns_Historiek_Met_Bankrekeningnummer(VoegBankrekeningnummerToeContext testContext)
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
    public void With_BankrekeningnummerWerdToegevoegd_Gebeurtenissen()
    {
        var nextId =
            _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Bankrekeningnummers.Max(
                x => x.BankrekeningnummerId
            ) + 1;

        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(BankrekeningnummerWerdToegevoegd)
        );
        gebeurtenisResponse.ShouldCompare(
            HistoriekGebeurtenisMapper.BankrekeningWerdToegevoegd(nextId, _testContext.CommandRequest),
            compareConfig: HistoriekComparisonConfig.Instance
        );
    }
}
