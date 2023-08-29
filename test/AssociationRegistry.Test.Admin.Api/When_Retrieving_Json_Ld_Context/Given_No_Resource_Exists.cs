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
public class Given_No_Resource_Exists
{
    private readonly AdminApiClient _adminApiClient;

    public Given_No_Resource_Exists(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
    }

    [Fact]
    public async Task Then_we_get_a_NotFound_response()
    {
        var fixture = new Fixture();
        var response = await _adminApiClient.GetJsonLdContext(fixture.Create<string>());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
