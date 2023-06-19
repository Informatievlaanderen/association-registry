namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Json_Ld_Context;

using AssociationRegistry.Admin.Api.Contexten;
using Fixtures;
using Framework;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_The_Resource_Exists
{
    private readonly AdminApiClient _adminApiClient;

    public Given_The_Resource_Exists(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
    }

    [Theory]
    [InlineData("detail-vereniging-context.json")]
    [InlineData("zoek-verenigingen-context.json")]
    public async Task Then_we_get_a_successful_response(string contextName)
    {
        var response = await _adminApiClient.GetJsonLdContext(contextName);
        response.Should().BeSuccessful();
    }

    [Theory]
    [InlineData("detail-vereniging-context.json")]
    [InlineData("zoek-verenigingen-context.json")]
    public async Task Then_the_context_json_is_returned(string contextName)
    {
        var response = await _adminApiClient.GetJsonLdContext(contextName);
        var json = await response.Content.ReadAsStringAsync();

        json.Should().BeEquivalentJson(JsonLdContexts.GetContext(contextName));
    }
}
