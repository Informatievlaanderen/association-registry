namespace AssociationRegistry.Test.E2E.When_Verwijder_Vertegenwoordiger_Uit_KBO.Beheer.Historiek;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(VerwijderVertegenwoordigerUitKBOCollection))]
public class Returns_Detail_With_Vertegenwoordiger : End2EndTest<HistoriekResponse>
{
    private readonly VerwijderVertegenwoordigerUitKBOContext _testContext;

    public Returns_Detail_With_Vertegenwoordiger(VerwijderVertegenwoordigerUitKBOContext testContext) : base(testContext.ApiSetup)
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
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdVerwijderdUitKBO));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdVerwijderdUitKBO(_testContext.Scenario.VertegenwoordigerWerdVerwijderdUitKBO),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
