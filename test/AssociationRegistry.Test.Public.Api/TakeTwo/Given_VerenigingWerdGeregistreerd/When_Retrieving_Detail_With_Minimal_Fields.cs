namespace AssociationRegistry.Test.Public.Api.TakeTwo.Given_VerenigingWerdGeregistreerd;

using System.Text.RegularExpressions;
using AssociationRegistry.Public.Api.Constants;
using Fixtures;
using Framework;
using TakeTwo;
using FluentAssertions;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class When_Retrieving_Detail_With_Minimal_Fields
{
    private readonly PublicApiClient _publicApiClient;
    private readonly string _vCode;

    public When_Retrieving_Detail_With_Minimal_Fields(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _vCode = fixture.VerenigingWerdGeregistreerdWithMinimalFieldsScenario.VCode;
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
        var responseMessage = await _publicApiClient.GetDetail(_vCode);

        var content = await responseMessage.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(When_Retrieving_Detail_With_Minimal_Fields)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
