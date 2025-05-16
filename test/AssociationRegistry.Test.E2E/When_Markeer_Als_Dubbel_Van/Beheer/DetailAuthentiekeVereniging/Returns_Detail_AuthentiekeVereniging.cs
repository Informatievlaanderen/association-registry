namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.DetailAuthentiekeVereniging;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using Framework.AlbaHost;
using FluentAssertions;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail_AuthentiekeVereniging : IClassFixture<MarkeerAlsDubbelVanContext>, IAsyncLifetime
{
    private readonly MarkeerAlsDubbelVanContext _context;
    private readonly ITestOutputHelper _helper;

    public Returns_Detail_AuthentiekeVereniging(MarkeerAlsDubbelVanContext context, ITestOutputHelper helper)
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
        Response.Vereniging.CorresponderendeVCodes.Should().Contain(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    [Fact]
    public void With_Status_Is_Actief()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Actief);
    }

    public DetailVerenigingResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetBeheerDetail(_context.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    public async ValueTask DisposeAsync()
    {
    }
}
