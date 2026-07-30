namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Zoeken.InStopzetting.When_Searching_An_Vereniging_InStopzetting;

using System.Net;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class Given_An_Vereniging_InStopzetting
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V085_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithInStopzetting_ForSearchOnInStopzetting _scenario;

    public Given_An_Vereniging_InStopzetting(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.AdminApiClient;
        _scenario =
            fixture.V085_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithInStopzetting_ForSearchOnInStopzetting;
    }

    [Fact]
    public async ValueTask With_Query_InStopzetting_Is_True_Then_Returns_Vereniging()
    {
        var response = await _adminApiClient.Search(q: $"vCode:{_scenario.VCode} AND inStopzetting:true");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var verenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

        verenigingenResponse!.Verenigingen.Single().InStopzetting.Should().BeTrue();
    }

    [Fact]
    public async ValueTask With_Query_InStopzetting_Is_False_Then_Returns_No_Vereniging()
    {
        var response = await _adminApiClient.Search(q: $"vCode:{_scenario.VCode} AND inStopzetting:false");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var verenigingenResponse = JsonConvert.DeserializeObject<SearchVerenigingenResponse>(content);

        verenigingenResponse!.Verenigingen.Should().BeEmpty();
    }
}
