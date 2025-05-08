namespace AssociationRegistry.Test.Public.Api.Given_an_Event_That_Is_Not_Handled;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using System.Text.RegularExpressions;
using templates;
using Vereniging;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class When_Searching_By_Name
{
    private readonly PublicApiClient _publicApiClient;
    private readonly VCode _vCode;
    private readonly V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario _scenario;

    public When_Searching_By_Name(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V004UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario;
        _vCode = _scenario.VCode;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario.Naam)).Should()
           .BeSuccessful();

    [Fact]
    public async ValueTask Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario.Naam);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario.Naam)
                          .WithVereniging(v => v.FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd));

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var response = await _publicApiClient.Search("stende");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate().FromQuery("stende"));
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var response = await _publicApiClient.Search("*stende*");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate().FromQuery("*stende*")
                                                                 .WithVereniging(
                                                                      v => v.FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd));

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var response = await _publicApiClient.Search("oostende");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate().FromQuery("oostende")
                                                                 .WithVereniging(
                                                                      v => v.FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd));

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var response = await _publicApiClient.Search(_vCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate().FromQuery(_vCode)
                                                                 .WithVereniging(
                                                                      v => v.FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd));

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async ValueTask Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var response = await _publicApiClient.Search("001");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate().FromQuery("001"));
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
