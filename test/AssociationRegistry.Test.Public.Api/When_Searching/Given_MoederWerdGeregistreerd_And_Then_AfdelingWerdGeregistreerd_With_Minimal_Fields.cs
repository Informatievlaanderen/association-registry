namespace AssociationRegistry.Test.Public.Api.When_Searching;

using AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;
using AssociationRegistry.Test.Public.Api.Framework;
using Fixtures;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly V009_MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;
    private readonly string _goldenMasterMoederVereniging;
    private readonly string _goldenMasterDochterVereniging;

    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields(GivenEventsFixture fixture)
    {
        _scenario = fixture.V009MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario;

        _publicApiClient = fixture.PublicApiClient;

        _goldenMasterMoederVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_vCode_searched_for_moeder)}");

        _goldenMasterDochterVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_vCode_searched_for_dochter)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_for_dochter()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vCode_searched_for_dochter()
    {
        var response = await _publicApiClient.Search($"vCode:{_scenario.VCode}");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterDochterVereniging
           .Replace("{{originalQuery}}", _scenario.VCode);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_for_moeder()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vCode_searched_for_moeder()
    {
        var response = await _publicApiClient.Search($"vCode:{_scenario.MoederVCode}");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterMoederVereniging
           .Replace("{{originalQuery}}", _scenario.MoederVCode);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
