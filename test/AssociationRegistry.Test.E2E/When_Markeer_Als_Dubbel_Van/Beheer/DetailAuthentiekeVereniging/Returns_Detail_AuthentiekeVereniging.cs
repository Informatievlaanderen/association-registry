namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.DetailAuthentiekeVereniging;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;


[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<DetailVerenigingResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext, ITestOutputHelper helper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient ,_testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode, headers: new RequestParameters().WithExpectedSequence(_testContext.VerenigingAanvaarddeDubbeleVereniging!.Sequence)).GetAwaiter().GetResult();

    [Fact]
    public void With_IsDubbelVan_VCode_Of_AndereFeitelijkeVerenigingWerdGeregistreerd()
    {
        Response.Vereniging.IsDubbelVan.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask With_DubbeleVereniging_In_CorresponderendeVCodes()
    {
        var tryCounter = 0;

        while (tryCounter < 20)
        {
            ++tryCounter;
            await Task.Delay(500);

            _helper.WriteLine($"Looking for CorresponderendeVCodes (try {tryCounter})...");
            if (Response.Vereniging.CorresponderendeVCodes.Any())
            {
                _helper.WriteLine("Found it!");
                break;
            }

            _helper.WriteLine("Did not find any CorresponderendeVCodes.");
        }
        Response.Vereniging.CorresponderendeVCodes.Should().Contain(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    [Fact]
    public void With_Status_Is_Actief()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Actief);
    }
}
