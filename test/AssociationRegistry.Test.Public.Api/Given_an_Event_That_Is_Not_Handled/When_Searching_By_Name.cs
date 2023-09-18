namespace AssociationRegistry.Test.Public.Api.Given_an_Event_That_Is_Not_Handled;

using System.Text.RegularExpressions;
using Fixtures;
using Framework;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class When_Searching_By_Name
{
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;
    private readonly VCode _vCode;

    private const string EmptyVerenigingenResponse =
        "{\"@context\":\"https://127.0.0.1:11003/v1/contexten/publiek/zoek-verenigingen-context.json\", \"verenigingen\": [], \"facets\": {\"hoofdactiviteitenVerenigingsloket\":[]}, \"metadata\": {\"pagination\": {\"totalCount\": 0,\"offset\": 0,\"limit\": 50}}}";

    public When_Searching_By_Name(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        var scenario = fixture.V004UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario;
        _vCode = scenario.VCode;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"{nameof(When_Searching_By_Name)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }


    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario.Naam)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario.Naam);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario.Naam);
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var response = await _publicApiClient.Search("stende");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var response = await _publicApiClient.Search("*stende*");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", "*stende*");
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var response = await _publicApiClient.Search("oostende");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", "oostende");
        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var response = await _publicApiClient.Search(_vCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", _vCode);
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

        var regex = new Regex(@"""facets"":\s*{\s*""hoofdactiviteitenVerenigingsloket"":(.|\s)*?""query"":"".*?(\/v1\/.+?)""");
        var regexResult = regex.Match(content);
        var urlFromFacets = regexResult.Groups[2].Value;

        var responseFromFacetsUrl = await _publicApiClient.HttpClient.GetAsync(urlFromFacets);
        var contentFromFacetsUrl = await responseFromFacetsUrl.Content.ReadAsStringAsync();

        const string expectedUrl = "/v1/verenigingen/zoeken?q=*dena*&facets.hoofdactiviteitenVerenigingsloket=BLA";
        contentFromFacetsUrl.Should().Contain(expectedUrl);
    }
}
