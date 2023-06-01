namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Framework;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_AfdelingWerdGeregistreerd
{
    private readonly V007_AfdelingWerdGeregistreerdScenario _scenario;
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;

    private const string EmptyVerenigingenResponse = "{\"verenigingen\": [], \"facets\": {\"hoofdactiviteitenVerenigingsloket\":[]}, \"metadata\": {\"pagination\": {\"totalCount\": 0,\"offset\": 0,\"limit\": 50}}}";

    public Given_AfdelingWerdGeregistreerd(GivenEventsFixture fixture)
    {
        _scenario = fixture.V007AfdelingWerdGeregistreerdScenario;
        _publicApiClient = fixture.PublicApiClient;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_AfdelingWerdGeregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_vCode_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", _scenario.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
