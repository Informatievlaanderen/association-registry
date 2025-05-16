namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using System.Net;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_Vereniging_Not_Found : IClassFixture<MarkeerAlsDubbelVanContext>, IAsyncLifetime
{
    private readonly MarkeerAlsDubbelVanContext _context;

    public Returns_Vereniging_Not_Found(MarkeerAlsDubbelVanContext context)
    {
        _context = context;
    }

    [Fact]
    public void Status_Code_Is_NotFound()
    {
        Response.Should().Be(HttpStatusCode.NotFound);
    }

    public HttpStatusCode Response { get; set; }

    public async Task InitializeAsync()
    {
        Response = _context.ApiSetup.PublicApiHost.GetPubliekDetailStatusCode(_context.VCode);
    }

    public async Task DisposeAsync()
    {
    }
}
