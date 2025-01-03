namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching;

using Framework.Fixtures;
using Framework.templates;
using Common.Scenarios.EventsInDb;
using Common.Extensions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_RoepnaamWerdGewijzigd
{
    private readonly V038_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_RoepnaamWerdGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V038VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens;
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
                                   .WithRoepnaam(_scenario.RoepnaamWerdGewijzigd.Roepnaam)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
