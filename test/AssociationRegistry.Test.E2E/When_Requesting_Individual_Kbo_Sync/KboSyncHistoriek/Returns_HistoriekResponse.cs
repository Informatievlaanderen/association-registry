namespace AssociationRegistry.Test.E2E.When_Requesting_Individual_Kbo_Sync.KboSyncHistoriek;

using Admin.Api.Verenigingen.KboSync.ResponseModels;
using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Events;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using When_Wijzig_Basisgegevens_Kbo;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using When_Stop_Vereniging;
using Xunit;

[Collection(nameof(IndividualKboSyncCollection))]
public class Returns_KboSyncHistoriekResponse : End2EndTest<KboSyncHistoriekResponse>
{
    private readonly IndividualKboSyncContext _testContext;
    private readonly ITestOutputHelper _outputHelper;

    public Returns_KboSyncHistoriekResponse(IndividualKboSyncContext testContext, ITestOutputHelper outputHelper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _outputHelper = outputHelper;
    }

    public override KboSyncHistoriekResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetKboSyncHistoriek(setup.SuperAdminHttpClient ,_testContext.VCode, outputHelper: _outputHelper).GetAwaiter().GetResult();

    [Fact(Skip = "Wait until we have a REAL record in the kbo event stream")]
    public void With_VCode()
    {
        Response.Where(x => x.VCode == _testContext.VCode)
                .Should()
                .HaveCount(1)
                .And.Subject
                .Single()
                .Beschrijving.Should()
                .Be("KBO Sync historiek");
    }
}
