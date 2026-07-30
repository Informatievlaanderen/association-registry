namespace AssociationRegistry.Test.Public.Api.When_Searching.InStopzetting.When_Searching_A_Vereniging_InStopzetting;

using System.Net;
using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_An_Erkende_Vereniging
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V026_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithInStopzetting_ForSearchOnInStopzetting _scenario;

    public Given_An_Erkende_Vereniging(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario =
            fixture.V026VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithInStopzettingForSearchOnInStopzetting;
    }

    [Fact]
    public async ValueTask With_Query_InStopzetting_Is_True_Then_Returns_Vereniging()
    {
        var response = await _publicApiClient.Search(q: $"vCode:{_scenario.VCode} AND inStopzetting:true");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var verenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

        verenigingenResponse!.Verenigingen.Single().InStopzetting.Should().BeTrue();
    }

    [Fact]
    public async ValueTask With_Query_InStopzetting_Is_False_Then_Returns_No_Vereniging()
    {
        var response = await _publicApiClient.Search(q: $"vCode:{_scenario.VCode} AND inStopzetting:false");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var verenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

        verenigingenResponse!.Verenigingen.Should().BeEmpty();
    }
}
