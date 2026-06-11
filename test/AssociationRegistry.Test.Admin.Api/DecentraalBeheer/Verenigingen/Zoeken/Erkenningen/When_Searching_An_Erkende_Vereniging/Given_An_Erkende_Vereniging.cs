namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Zoeken.Erkenningen.When_Searching_An_Erkende_Vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class Given_An_Erkende_Vereniging
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V084_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithActieveErkenning_ForSearchOnErkenning _scenario;

    public Given_An_Erkende_Vereniging(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.AdminApiClient;
        _scenario =
            fixture.V084VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningForSearchOnErkenning;
    }

    [Fact]
    public async ValueTask With_Query_Erkend_Is_True_Then_Returns_Vereniging()
    {
        var response = await _adminApiClient.Search(q: $"vCode:{_scenario.VCode} AND isErkend:true");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var verenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

        verenigingenResponse!.Verenigingen.Single().IsErkend.Should().BeTrue();
    }

    [Fact]
    public async ValueTask With_Query_Erkend_Is_False_Then_Returns_No_Vereniging()
    {
        var response = await _adminApiClient.Search(q: $"vCode:{_scenario.VCode} AND isErkend:false");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var verenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

        verenigingenResponse!.Verenigingen.Should().BeEmpty();
    }
}
