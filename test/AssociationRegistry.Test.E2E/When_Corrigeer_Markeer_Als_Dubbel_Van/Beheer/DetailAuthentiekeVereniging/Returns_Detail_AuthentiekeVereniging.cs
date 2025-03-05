namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van.Beheer.DetailAuthentiekeVereniging;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Requests;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail_AuthentiekeVereniging : End2EndTest<CorrigeerMarkeringAlsDubbelVanContext, NullRequest,
    DetailVerenigingResponse>
{
    private readonly CorrigeerMarkeringAlsDubbelVanContext _context;
    private readonly ITestOutputHelper _helper;

    public Returns_Detail_AuthentiekeVereniging(CorrigeerMarkeringAlsDubbelVanContext context, ITestOutputHelper helper) : base(context)
    {
        _context = context;
        _helper = helper;
    }

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

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerDetail(_context.Scenario.AuthentiekeVereniging.VCode);
}
