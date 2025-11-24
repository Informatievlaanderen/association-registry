namespace AssociationRegistry.Test.E2E.When_Wijzig_Vertegenwoordiger_In_KBO.Beheer.Historiek;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(WijzigVertegenwoordigerInKBOCollection))]
public class Returns_Detail_With_Vertegenwoordiger : End2EndTest<HistoriekResponse>
{
    private readonly WijzigVertegenwoordigerInKBOContext _testContext;

    public Returns_Detail_With_Vertegenwoordiger(WijzigVertegenwoordigerInKBOContext testContext) : base(testContext.ApiSetup)
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
    public void With_VertegenwoordigerWerdGewijzigdInKBO_Gebeurtenissen()
    {
        var gebeurtenisResponse = Response.Gebeurtenissen.SingleOrDefault(x => x.Gebeurtenis == nameof(VertegenwoordigerWerdGewijzigdInKBO));
        gebeurtenisResponse.ShouldCompare(HistoriekGebeurtenisMapper.VertegenwoordigerWerdGewijzigdInKBO(_testContext.Scenario.VertegenwoordigerWerdGewijzigdInKBO),
                                        compareConfig: HistoriekComparisonConfig.Instance);
    }
}
