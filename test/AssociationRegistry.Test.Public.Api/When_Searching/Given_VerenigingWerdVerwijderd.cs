namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingWerdVerwijderd
{
    private readonly V018_FeitelijkeVerenigingWerdVerwijderdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingWerdVerwijderd(GivenEventsFixture fixture)
    {
        _scenario = fixture.V018_FeitelijkeVerenigingWerdVerwijderdScenario;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_no_vereniging_matching_the_vCode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = new ZoekVerenigingenResponseTemplate();
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
