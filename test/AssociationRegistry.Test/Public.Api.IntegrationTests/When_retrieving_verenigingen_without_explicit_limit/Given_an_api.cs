using AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_verenigingen_without_explicit_limit;

[Collection(VerenigingPublicApiCollection.Name)]
public class Given_an_api : IClassFixture<VerenigingPublicApiFixture>
{
    private readonly HttpClient _httpClient;
    public Given_an_api(VerenigingPublicApiFixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _httpClient.GetAsync("/v1/verenigingen")).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_list_verenigingen_response()
    {
        var responseMessage = await _httpClient.GetAsync("/v1/verenigingen");
        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_an_api)}_{nameof(Then_we_get_a_list_verenigingen_response)}");

        var deserializedContent = JToken.Parse(content);
        var deserializedGoldenMaster = JToken.Parse(goldenMaster);

        deserializedContent.Should().BeEquivalentTo(deserializedGoldenMaster);
    }
}
