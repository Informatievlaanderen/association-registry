namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Common.Extensions;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using System.Threading.Tasks;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class With_Dot_Not_Matching
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V019_Vereniging19ForSearchScenario _scenario;
    private readonly string _query = "N.A.S.A.";
    private readonly string _query2 = "H.A.H.A.";

    public With_Dot_Not_Matching(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V019Vereniging19ForSearchScenario;
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

    [Fact]
    public async Task Then_we_get_a_successful_response2()
        => (await _publicApiClient.Search(_query2)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched2()
    {
        var response = await _publicApiClient.Search(_query2);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster =
            new ZoekVerenigingenResponseTemplate()
               .FromQuery(_query2)
               .WithVereniging(
                    v => v
                       .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
