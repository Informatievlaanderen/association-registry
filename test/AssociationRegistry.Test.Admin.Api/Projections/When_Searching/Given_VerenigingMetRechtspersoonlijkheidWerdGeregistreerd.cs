namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Framework.templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens;
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
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
