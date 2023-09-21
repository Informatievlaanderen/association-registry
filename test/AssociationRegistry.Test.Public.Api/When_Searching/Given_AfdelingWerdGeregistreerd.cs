namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using Framework;
using FluentAssertions;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_AfdelingWerdGeregistreerd
{
    private readonly V007_AfdelingWerdGeregistreerdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_AfdelingWerdGeregistreerd(GivenEventsFixture fixture)
    {
        _scenario = fixture.V007AfdelingWerdGeregistreerdScenario;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster =
            new ZoekVerenigingenResponseTemplate()
               .FromQuery(_scenario.VCode)
               .WithVereniging()
               .FromEvent(_scenario.AfdelingWerdGeregistreerd)
               .And()
               .Build();

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
