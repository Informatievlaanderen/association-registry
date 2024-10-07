namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using System.Text.RegularExpressions;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields
{
    private readonly V001_FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields(GivenEventsFixture fixture)
    {
        _scenario = fixture.V001FeitelijkeVerenigingWerdGeregistreerdScenario;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var query = "Feestcommittee Oudenaarde";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    private string GoldenMaster(string query)
        => new ZoekVerenigingenResponseTemplate()
          .FromQuery(query)
          .WithVereniging(
               v => v
                  .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
           );

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var query = "dena";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var query = "*dena*";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var query = "oudenaarde";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var query = _scenario.VCode;
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var response = await _publicApiClient.Search("001");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_werkingsgebied()
    {
        var query = "werkingsgebieden.code:BE25";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_a_sub_werkingsgebied()
    {
        var query = "werkingsgebieden.code:BE255";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
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
