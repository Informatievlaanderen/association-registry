namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Microsoft.Net.Http.Headers;
using System.Net;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V017_AfdelingWerdGeregistreerd_WithMinimalFields_And_Registered_Moeder _scenario;

    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V017AfdelingWerdGeregistreerdWithMinimalFieldsAndRegisteredMoeder;
        _adminApiClient = fixture.DefaultClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, _scenario.Result.Sequence))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, long.MaxValue))
          .StatusCode
          .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_afdeling_response()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.AfdelingWerdGeregistreerd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip);

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_we_get_a_detail_moeder_response()
    {
        var response = await _adminApiClient.GetDetail(_scenario.MoederWerdGeregistreerd.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.MoederWerdGeregistreerd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip)
                      .HeeftAfdeling(_scenario.AfdelingWerdGeregistreerd.VCode, _scenario.AfdelingWerdGeregistreerd.Naam);

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_it_returns_an_etag_header()
    {
        var afdelingResponse = await _adminApiClient.GetDetail(_scenario.VCode);
        afdelingResponse.Headers.ETag.Should().NotBeNull();

        var etag = afdelingResponse.Headers.GetValues(HeaderNames.ETag).ToList().Should().ContainSingle().Subject;
        etag.Should().StartWith("W/\"").And.EndWith("\"");
    }
}
