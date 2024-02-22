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
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V062VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndSynced;
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
    public async Task Then_we_get_a_detail_response()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip)
                      .WithNaam(_scenario.NaamWerdGewijzigdInKbo.Naam)
                      .WithKorteNaam(_scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam);

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_it_returns_an_etag_header()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        response.Headers.ETag.Should().NotBeNull();

        var etag = response.Headers.GetValues(HeaderNames.ETag).ToList().Should().ContainSingle().Subject;
        etag.Should().StartWith("W/\"").And.EndWith("\"");
    }
}
