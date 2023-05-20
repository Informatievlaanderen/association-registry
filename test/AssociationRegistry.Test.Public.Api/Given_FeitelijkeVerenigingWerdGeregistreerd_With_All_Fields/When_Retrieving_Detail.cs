namespace AssociationRegistry.Test.Public.Api.Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields;

using System.Text.RegularExpressions;
using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Test.Public.Api.Fixtures;
using AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;
using AssociationRegistry.Test.Public.Api.Framework;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class When_Retrieving_Detail
{
    private readonly PublicApiClient _publicApiClient;
    private readonly string _vCode;

    public When_Retrieving_Detail(GivenEventsFixture fixture)
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
            $"{nameof(When_Retrieving_Detail)}_{nameof(Then_we_get_a_detail_vereniging_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}

