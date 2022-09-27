using AssociationRegistry.Public.Api;
using AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_verenigingen_without_explicit_limit;

using AssociationRegistry.Public.Api.Constants;

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
    public async Task Then_we_get_json_ld_as_content_type()
        => (await _httpClient.GetAsync("/v1/verenigingen")).Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);

    [Fact]
    public async Task Then_we_get_a_list_verenigingen_response()
    {
        var responseMessage = await _httpClient.GetAsync("/v1/verenigingen");
        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_an_api)}_{nameof(Then_we_get_a_list_verenigingen_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task Then_we_get_a_successful_detail_link_response()
    {
        var responseMessage = await _httpClient.GetAsync("/v1/verenigingen");
        var content = await responseMessage.Content.ReadAsStringAsync();
        var deserializedContent = JToken.Parse(content);

        var detailLink = deserializedContent.SelectToken("$.verenigingen[0].links[0].href")!.Value<string>();

        (await _httpClient.GetAsync(detailLink)).Should().BeSuccessful();
    }
}
