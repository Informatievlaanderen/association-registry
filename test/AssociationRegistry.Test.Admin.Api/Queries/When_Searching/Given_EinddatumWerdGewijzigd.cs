<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Searching/Given_EinddatumWerdGewijzigd.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Searching;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Searching/Given_EinddatumWerdGewijzigd.cs
using Framework.Fixtures;
using Framework.templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_EinddatumWerdGewijzigd
{
    private readonly V064_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_EinddatumWerdGewijzigd _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_EinddatumWerdGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V064VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndEinddatumWerdGewijzigd;
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
                                   .WithEinddatum(_scenario.EinddatumWerdGewijzigd.Einddatum)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
