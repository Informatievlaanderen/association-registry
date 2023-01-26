namespace AssociationRegistry.Test.Public.Api.Given_VerenigingWerdGeregistreerd_With_All_Fields;

using System.Text.RegularExpressions;
using AssociationRegistry.Test.Public.Api.Fixtures;
using AssociationRegistry.Test.Public.Api.Framework;
using Fixtures.GivenEvents;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class When_Searching_By_Name
{
    private readonly VerenigingWerdGeregistreerdScenario _scenario;
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;

    private const string EmptyVerenigingenResponse = "{\"verenigingen\": [], \"facets\": {\"hoofdactiviteiten\":[]}, \"metadata\": {\"pagination\": {\"totalCount\": 0,\"offset\": 0,\"limit\": 50}}}";

    public When_Searching_By_Name(GivenEventsFixture fixture)
    {
        _scenario = fixture.VerenigingWerdGeregistreerdScenario;
        _publicApiClient = fixture.PublicApiClient;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"{nameof(When_Searching_By_Name)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(_scenario.Naam);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", _scenario.Naam);
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var response = await _publicApiClient.Search("dena");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var response = await _publicApiClient.Search("*dena*");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", "*dena*");
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var response = await _publicApiClient.Search("oudenaarde");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", "oudenaarde");
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", _scenario.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var response = await _publicApiClient.Search("001");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }

    [Fact]
    public async Task? When_Navigating_To_A_Hoofdactiviteit_Facet_Then_it_is_retrieved()
    {
        var response = await _publicApiClient.Search("*dena*");
        var content = await response.Content.ReadAsStringAsync();

        var regex = new Regex(@"""facets"":\s*{\s*""hoofdactiviteiten"":(.|\s)*?""query"":"".*?(\/v1\/.+?)""");
        var regexResult = regex.Match(content);
        var urlFromFacets = regexResult.Groups[2].Value;

        var responseFromFacetsUrl = await _publicApiClient.HttpClient.GetAsync(urlFromFacets);
        var contentFromFacetsUrl = await responseFromFacetsUrl.Content.ReadAsStringAsync();

        const string expectedUrl = "/v1/verenigingen/zoeken?q=*dena*&facets.hoofdactiviteiten=BWWC";
        contentFromFacetsUrl.Should().Contain(expectedUrl);
    }
}
