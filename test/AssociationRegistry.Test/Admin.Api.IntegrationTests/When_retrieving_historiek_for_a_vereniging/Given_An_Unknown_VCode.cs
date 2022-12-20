namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_retrieving_historiek_for_a_vereniging;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;

public class Given_An_Unknown_Vereniging_Fixture : AdminApiFixture
{
    public const string VCode = "v9999999";

    public Given_An_Unknown_Vereniging_Fixture() : base(nameof(Given_An_Unknown_Vereniging_Fixture))
    {
    }

}

public class Given_An_Unknown_Vereniging : IClassFixture<Given_An_Unknown_Vereniging_Fixture>
{
    private const string VCode = Given_An_Unknown_Vereniging_Fixture.VCode;
    private readonly HttpClient _httpClient;

    public Given_An_Unknown_Vereniging(Given_An_Unknown_Vereniging_Fixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task Then_we_get_a_404()
    {
        var response = await _httpClient.GetAsync($"/v1/verenigingen/{VCode}/historiek");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
