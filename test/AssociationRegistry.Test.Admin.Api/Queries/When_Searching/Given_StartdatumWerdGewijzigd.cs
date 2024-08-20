<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Searching/Given_StartdatumWerdGewijzigd.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Searching;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Searching/Given_StartdatumWerdGewijzigd.cs
using Framework.Fixtures;
using Framework.templates;
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
