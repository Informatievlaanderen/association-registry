namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_StartdatumWerdGewijzigd
{
    private readonly V063_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_StartdatumWerdGewijzigd _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_StartdatumWerdGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V063VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndStartdatumWerdGewijzigd;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                                   .WithStartdatum(_scenario.StartdatumWerdGewijzigd.Startdatum)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
