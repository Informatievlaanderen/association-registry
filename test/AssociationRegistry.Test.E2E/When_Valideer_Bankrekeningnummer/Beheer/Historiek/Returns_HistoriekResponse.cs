namespace AssociationRegistry.Test.E2E.When_Valideer_Bankrekeningnummer.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(ValideerBankrekeningnummerCollection))]
public class Returns_Historiek_Met_Bankrekeningnummer : End2EndTest<HistoriekResponse>
{
    private readonly ValideerBankrekeningnummerContext _testContext;

    public Returns_Historiek_Met_Bankrekeningnummer(ValideerBankrekeningnummerContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerHistoriek(setup.AdminHttpClient, _testContext.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

    [Fact]
    public void With_BankrekeningnummerWerdToegevoegd_Gebeurtenissen()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.BankrekeningnummerWerdGevalideerd(_testContext.Scenario.BankrekeningnummerWerdToegevoegd),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
