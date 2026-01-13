namespace AssociationRegistry.Test.E2E.When_Verwijder_Bankrekeningnummer_Uit_KBO.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(BankrekeningnummerWerdVerwijderdUitKBOCollection))]
public class Returns_Historiek_Of_Verwijderde_Bankrekeningnummer : End2EndTest<HistoriekResponse>
{
    private readonly VerwijderBankrekeningnummerUitKBOContext _testContext;

    public Returns_Historiek_Of_Verwijderde_Bankrekeningnummer(VerwijderBankrekeningnummerUitKBOContext testContext) : base(testContext.ApiSetup)
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
    public void With_VertegenwoordigerWerdOvergenomenUitKBO_Gebeurtenissen()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(BankrekeningnummerWerdVerwijderdUitKBO));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.BankrekeningnummerWerdVerwijderdUitKBO(_testContext.Scenario.BankrekeningnummerWerdVerwijderdUitKBO),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
