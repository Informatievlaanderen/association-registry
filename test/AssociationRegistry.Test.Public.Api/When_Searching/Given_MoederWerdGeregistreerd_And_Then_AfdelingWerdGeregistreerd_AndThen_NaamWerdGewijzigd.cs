namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using templates;
using Test.Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_AndThen_NaamWerdGewijzigd
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V018_AfdelingWerdGeregistreerd_MetBestaandeMoeder_VoorNaamWerdGewijzigd _scenario;

    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_AndThen_NaamWerdGewijzigd(GivenEventsFixture fixture)
    {
        _scenario = fixture.V018AfdelingWerdGeregistreerdMetBestaandeMoederVoorNaamWerdGewijzigd;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_for_dochter()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vCode_searched_for_dochter()
    {
        var response = await _publicApiClient.Search($"vCode:{_scenario.VCode}");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.AfdelingWerdGeregistreerd)
                                   .WithNaam(_scenario.NaamWerdGewijzigd.Naam)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_for_moeder()
        => (await _publicApiClient.Search(_scenario.VCodeMoeder)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vCode_searched_for_moeder()
    {
        var response = await _publicApiClient.Search($"vCode:{_scenario.VCodeMoeder}");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCodeMoeder)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.MoederWerdGeregistreerd)
                                   .HeeftAfdeling(_scenario.AfdelingWerdGeregistreerd.VCode, _scenario.NaamWerdGewijzigd.Naam)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
