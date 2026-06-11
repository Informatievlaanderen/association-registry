namespace AssociationRegistry.Test.Public.Api.When_Searching.Erkenningen.When_Searching_An_Erkende_Vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
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
    private readonly V025_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningForSearchOnErkenningScenario _scenario;

    public Given_An_Erkende_Vereniging(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario =
            fixture.V025VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningForSearchOnErkenningScenario;
    }

    [Fact]
    public async ValueTask With_Query_Erkend_Is_True_Then_Returns_Vereniging()
    {
        var response = await _publicApiClient.Search(q: $"vCode:{_scenario.VCode} AND isErkend:true");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var verenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

        verenigingenResponse!.Verenigingen.Single().IsErkend.Should().BeTrue();
    }

    [Fact]
    public async ValueTask With_Query_Erkend_Is_False_Then_Returns_No_Vereniging()
    {
        var response = await _publicApiClient.Search(q: $"vCode:{_scenario.VCode} AND isErkend:false");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var verenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

        verenigingenResponse!.Verenigingen.Should().BeEmpty();
    }
}
