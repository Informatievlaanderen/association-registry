namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures.GivenEvents;
using Framework;
using Fixtures;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly V009_MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields(GivenEventsFixture fixture)
    {
        _scenario = fixture.V009MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario;

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
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_for_moeder()
        => (await _publicApiClient.Search(_scenario.MoederVCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vCode_searched_for_moeder()
    {
        var response = await _publicApiClient.Search($"vCode:{_scenario.MoederVCode}");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.MoederVCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.MoederWerdGeregistreerd)
                                   .HeeftAfdeling(_scenario.AfdelingWerdGeregistreerd.VCode, _scenario.AfdelingWerdGeregistreerd.Naam)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
