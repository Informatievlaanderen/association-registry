namespace AssociationRegistry.Test.E2E.Erkenningen.When_Verwijder_Erkenning.Publiek.Detail;

using FluentAssertions;
using Framework.AlbaHost;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(VerwijderErkenningCollection))]
public class Returns_Detail_With_Geregistreerde_Erkenning : IAsyncLifetime
{
    private readonly VerwijderErkenningContext _context;

    public Returns_Detail_With_Geregistreerde_Erkenning(VerwijderErkenningContext context)
    {
        _context = context;
    }

    [Fact]
    public void JsonContentMatches()
    {
        Response.Vereniging.Erkenningen.Should().BeEmpty();
    }

    public PubliekVerenigingDetailResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = await _context.ApiSetup.PublicApiHost.GetPubliekDetail(_context.VCode);
    }

    public async ValueTask DisposeAsync() { }
}
