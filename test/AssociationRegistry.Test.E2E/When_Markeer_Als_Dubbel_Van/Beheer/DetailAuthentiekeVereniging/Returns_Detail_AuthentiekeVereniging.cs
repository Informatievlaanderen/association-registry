namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.DetailAuthentiekeVereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using FluentAssertions;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Detail_AuthentiekeVereniging : IClassFixture<MarkeerAlsDubbelVanContext>, IAsyncLifetime
{
    private readonly MarkeerAlsDubbelVanContext _context;

    public Returns_Detail_AuthentiekeVereniging(MarkeerAlsDubbelVanContext context)
    {
        _context = context;
    }

    [Fact]
    public void With_IsDubbelVan_VCode_Of_AndereFeitelijkeVerenigingWerdGeregistreerd()
    {
        Response.Vereniging.IsDubbelVan.Should().BeEmpty();
    }

    [Fact]
    public void With_DubbeleVereniging_In_CorresponderendeVCodes()
    {
        Response.Vereniging.CorresponderendeVCodes.Should().Contain(_context.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    [Fact]
    public void With_Status_Is_Actief()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Actief);
    }

    public DetailVerenigingResponse Response { get; set; }

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetBeheerDetail(_context.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    public async Task DisposeAsync()
    {
    }
}
