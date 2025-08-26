namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Beheer.DetailAuthentiekeVereniging;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class Returns_Detail_With_Dubbel_Van : End2EndTest<DetailVerenigingResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Returns_Detail_With_Dubbel_Van(CorrigeerMarkeringAlsDubbelVanContext testContext, ITestOutputHelper helper) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient ,_testContext.Scenario.AuthentiekeVereniging.VCode, headers:
                                              new RequestParameters()
                                                 .WithExpectedSequence(_testContext.AanvaarddeCorrectieDubbeleVereniging!.Sequence));

    [Fact]
    public void With_IsDubbelVan_VCode_Of_AndereFeitelijkeVerenigingWerdGeregistreerd()
    {
        Response.Vereniging.IsDubbelVan.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask With_No_DubbeleVereniging_In_CorresponderendeVCodes()
    {
        var tryCounter = 0;

        while (tryCounter < 20)
        {
            ++tryCounter;
            await Task.Delay(500);

            _helper.WriteLine($"Looking for CorresponderendeVCodes to be set to empty (try {tryCounter})...");

            if (!Response.Vereniging.CorresponderendeVCodes.Any())
            {
                _helper.WriteLine("CorresponderendeVCodes are cleared!");

                break;
            }

            _helper.WriteLine("Still has CorresponderendeVCodes, so this means the event is never fired.");
        }

        Response.Vereniging.CorresponderendeVCodes.Should().BeEmpty();
    }

    [Fact]
    public void With_Status_Is_Actief()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Actief);
    }
}
