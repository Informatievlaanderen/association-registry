namespace AssociationRegistry.Test.Admin.Api.When_Searching;

using Common.Extensions;
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
public class Given_A_Vereniging_Has_Been_Removed
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V059_FeitelijkeVerenigingWerdGeregistreerd_AndRemoved _scenario;

    public Given_A_Vereniging_Has_Been_Removed(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.SuperAdminApiClient;
        _scenario = fixture.V059FeitelijkeVerenigingWerdGeregistreerdAndRemoved;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_no_vereniging_matching_the_VCode_searched()
    {
        var query = $"vCode:{_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode}";
        var response = await _adminApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
           .FromQuery(query);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
