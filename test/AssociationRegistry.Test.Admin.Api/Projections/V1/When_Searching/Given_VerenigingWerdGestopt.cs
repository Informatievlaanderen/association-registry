namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching;

using AssociationRegistry.Admin.Schema.Constants;
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
public class Given_VerenigingWerdGestopt
{
    private readonly V041_FeitelijkeVerenigingWerdGestopt _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_VerenigingWerdGestopt(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V041FeitelijkeVerenigingWerdGestopt;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                                   .WithEinddatum(_scenario.EinddatumWerdGewijzigd.Einddatum)
                                   .WithStatus(VerenigingStatus.Gestopt));

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
