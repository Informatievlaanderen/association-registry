namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using System.Net;
using templates;
using Test.Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_AndThen_NaamWerdGewijzigd
{
    private readonly PublicApiClient _apiClient;
    private readonly V018_AfdelingWerdGeregistreerd_MetBestaandeMoeder_VoorNaamWerdGewijzigd _scenario;

    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_AndThen_NaamWerdGewijzigd(GivenEventsFixture fixture)
    {
        _scenario = fixture.V018AfdelingWerdGeregistreerdMetBestaandeMoederVoorNaamWerdGewijzigd;
        _apiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _apiClient.GetDetail(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _apiClient.GetDetail(_scenario.VCode))
          .Should().BeSuccessful();


    [Fact]
    public async Task Then_we_get_a_detail_afdeling_response()
    {
        var response = await _apiClient.GetDetail(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.AfdelingWerdGeregistreerd)
                      .WithNaam(_scenario.NaamWerdGewijzigd.Naam)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip);

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_we_get_a_detail_moeder_response()
    {
        var response = await _apiClient.GetDetail(_scenario.MoederWerdGeregistreerd.VCode);

        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.MoederWerdGeregistreerd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip)
                      .HeeftAfdeling(_scenario.AfdelingWerdGeregistreerd.VCode, _scenario.NaamWerdGewijzigd.Naam);

        content.Should().BeEquivalentJson(expected);
    }
}
