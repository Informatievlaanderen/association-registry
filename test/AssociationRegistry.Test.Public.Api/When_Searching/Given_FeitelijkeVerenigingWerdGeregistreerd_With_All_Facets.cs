namespace AssociationRegistry.Test.Public.Api.When_Searching;

using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;
using HoofdactiviteitVerenigingsloket = Vereniging.HoofdactiviteitVerenigingsloket;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Facets
{
    private readonly V024_FeitelijkeVerenigingWerdGeregistreerdWithAllFacetsScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Facets(GivenEventsFixture fixture)
    {
        _scenario = fixture.V024FeitelijkeVerenigingWerdGeregistreerdWithAllFacetsScenario;
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async ValueTask? Then_The_Amount_Of_Facets_Should_Be_The_Total_Count_Of_Facets()
    {
        var response = await _publicApiClient.Search("*");
        var content = await response.Content.ReadAsStringAsync();

       var searchVerenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

       searchVerenigingenResponse!.Facets!.HoofdactiviteitenVerenigingsloket.Length.Should()
                                 .Be(HoofdactiviteitVerenigingsloket.HoofdactiviteitenVerenigingsloketCount);
    }
}
