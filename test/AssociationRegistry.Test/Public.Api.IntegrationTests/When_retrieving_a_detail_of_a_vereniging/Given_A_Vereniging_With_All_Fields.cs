namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_a_detail_of_a_vereniging;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using FluentAssertions;
using Xunit;

[Collection(VerenigingPublicApiCollection.Name)]
public class Given_A_Vereniging_With_All_Fields : IClassFixture<StaticPublicApiFixture>
{
    private readonly HttpClient _httpClient;

    public Given_A_Vereniging_With_All_Fields(StaticPublicApiFixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _httpClient.GetAsync("/v1/verenigingen/static/V1234567");
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _httpClient.GetAsync("/v1/verenigingen/static/V1234567");
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var responseMessage = await _httpClient.GetAsync("/v1/verenigingen/static/V1234567");

        var content = await responseMessage.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_A_Vereniging_With_All_Fields)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
