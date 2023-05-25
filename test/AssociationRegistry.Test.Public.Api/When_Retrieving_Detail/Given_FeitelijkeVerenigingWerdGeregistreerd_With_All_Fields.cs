namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using System.Text.RegularExpressions;
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
public class Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields
{
    private readonly PublicApiClient _publicApiClient;
    private readonly string _vCode;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _vCode = fixture.V001FeitelijkeVerenigingWerdGeregistreerdScenario.VCode;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetDetail(_vCode);
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_vCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var response = await _publicApiClient.GetDetail(_vCode);

        var content = await response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
