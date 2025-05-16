namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using System.Text.RegularExpressions;
using templates;
using Xunit;

[Collection(nameof(PublicApiCollection))]
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
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async ValueTask Then_we_retrieve_one_vereniging_matching_the_name_searched()
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
               v =>
               {
                   var vereniging = v.FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd);

                   foreach (var werkingsgebied in _scenario.WerkingsgebiedenWerdenBepaald.Werkingsgebieden)
                       vereniging.WithWerkingsgebied(werkingsgebied.Code, werkingsgebied.Naam);

                   return vereniging;
               });

    [Fact]
    public async ValueTask Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var query = "dena";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var query = "*dena*";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var query = "oudenaarde";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var query = _scenario.VCode;
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var response = await _publicApiClient.Search("0001");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_Sequence()
    {
        var response = await _publicApiClient.Search("0000002");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_by_its_werkingsgebied()
    {
        var query = "werkingsgebieden.code:BE25";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Theory]
    [InlineData("NVT")]
    [InlineData("BE25")]
    [InlineData("BE255")]
    public async Task? Then_one_vereniging_is_not_retrieved_by_a_another_werkingsgebied(string werkingsgebiedCode)
    {
        var query = "werkingsgebieden.code:BE255";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_by_one_of_the_werkingsgebieden()
    {
        var query = "werkingsgebieden.code:(BE OR BE25 OR BE21 OR BE212)";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_not_retrieved_if_none_of_the_werkingsgebieden_match()
    {
        var query = "werkingsgebieden.code:(BE OR BE2 OR BE21 OR BE212)";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_if_none_of_the_werkingsgebieden_match()
    {
        var query = "werkingsgebieden.code:(BE25 AND BE25535003)";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GoldenMaster(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_not_retrieved_if_not_all_of_the_werkingsgebieden_match()
    {
        var query = "werkingsgebieden.code:(BE25 AND BE2)";
        var response = await _publicApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async ValueTask When_Navigating_To_A_Hoofdactiviteit_Facet_Then_it_is_retrieved()
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
