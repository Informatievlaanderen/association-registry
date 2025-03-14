namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
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

    // TODO: or-2794
    [Fact(Skip = "Known bug: use update document for beheer zoeken to fix this.")]
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
