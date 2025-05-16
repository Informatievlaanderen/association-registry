namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Common.Extensions;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class With_Accents_Not_Matching
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V020_Vereniging20ForSearchScenario _scenario;
    private readonly string _query = "nôôït";

    public With_Accents_Not_Matching(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V020Vereniging20ForSearchScenario;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_query)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(_query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster =
            new ZoekVerenigingenResponseTemplate()
               .FromQuery(_query)
               .WithVereniging(
                    v => v
                       .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
