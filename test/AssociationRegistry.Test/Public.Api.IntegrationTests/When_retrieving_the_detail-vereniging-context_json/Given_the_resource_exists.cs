namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_the_detail_vereniging_context_json;

using AssociationRegistry.Public.Api.json_ld_contexts;
using Fixtures;
using FluentAssertions;
using Xunit;

[Collection(VerenigingPublicApiCollection.Name)]
public class Given_the_resource_exists : IClassFixture<VerenigingPublicApiFixture>
{
    private readonly HttpClient _httpClient;

    public Given_the_resource_exists(VerenigingPublicApiFixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _httpClient.GetAsync("/v1/contexten/detail-vereniging-context.json");
        response.Should().BeSuccessful();
    }

    [Theory]
    [InlineData("detail-vereniging-context.json")]
    [InlineData("list-verenigingen-context.json")]
    public async Task Then_the_context_json_is_returned(string contextName)
    {
        var response = await _httpClient.GetAsync($"/v1/contexten/{contextName}");
        var json = await response.Content.ReadAsStringAsync();

        json.Should().BeEquivalentJson(JsonLdContexts.GetContext(contextName));
    }
}
