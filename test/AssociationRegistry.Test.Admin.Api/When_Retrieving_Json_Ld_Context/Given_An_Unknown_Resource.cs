namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Json_Ld_Context;

using AutoFixture;
using Fixtures;
using FluentAssertions;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Unknown_Resource
{
    private readonly AdminApiClient _adminApiClient;

    public Given_An_Unknown_Resource(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var fixture = new Fixture();
        var response = await _adminApiClient.GetJsonLdContext(fixture.Create<string>());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
