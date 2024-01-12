namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail_vereniging_context_json;

using AssociationRegistry.Public.Api.Contexten;
using Fixtures;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(VerenigingPublicApiCollection.Name)]
[Category("PublicApi")]
[IntegrationTest]
public class Given_The_Resource_Exists : IClassFixture<StaticPublicApiFixture>
{
    private readonly HttpClient _httpClient;

    public Given_The_Resource_Exists(StaticPublicApiFixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _httpClient.GetAsync("/v1/contexten/publiek/detail-vereniging-context.json");
        response.Should().BeSuccessful();
    }

    [Theory]
    [InlineData("detail-vereniging-context.json")]
    [InlineData("zoek-verenigingen-context.json")]
    public async Task Then_the_context_json_is_returned(string contextName)
    {
        var response = await _httpClient.GetAsync($"/v1/contexten/publiek/{contextName}");
        var json = await response.Content.ReadAsStringAsync();

        json.Should().BeEquivalentJson(JsonLdContexts.GetContext(folder: "publiek", contextName));
    }
}
