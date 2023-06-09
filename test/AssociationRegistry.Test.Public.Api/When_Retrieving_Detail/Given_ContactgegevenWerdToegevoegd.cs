namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_ContactgegevenWerdToegevoegd
{
    private readonly string _vCode;
    private readonly PublicApiClient _publicApiClient;
    private readonly HttpResponseMessage _response;

    public Given_ContactgegevenWerdToegevoegd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _vCode = fixture.V005ContactgegevenWerdToegevoegdScenario.AfdelingVCode;
        _response = _publicApiClient.GetDetail(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_vCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_ContactgegevenWerdToegevoegd)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
