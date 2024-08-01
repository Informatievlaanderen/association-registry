namespace AssociationRegistry.Test.Admin.Api.When_Searching;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using templates;
using Test.Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_Vereniging_Werd_Uitgeschreven_Uit_Publieke_Datastroom
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V079_FeitelijkeVerenigingWerdUitgeschrevenUitPubliekeDatastroom_And_NaamGewijzigdScenario _scenario;

    public Given_A_Vereniging_Werd_Uitgeschreven_Uit_Publieke_Datastroom(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V079FeitelijkeVerenigingWerdUitgeschrevenUitPubliekeDatastroomAndNaamGewijzigd;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_a_vereniging_matching_the_VCode_searched()
    {
        var query = $"vCode:{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}";
        var response = await _adminApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
           .FromQuery(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
