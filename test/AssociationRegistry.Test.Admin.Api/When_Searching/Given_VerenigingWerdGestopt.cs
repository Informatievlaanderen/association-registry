namespace AssociationRegistry.Test.Admin.Api.When_Searching;

using AssociationRegistry.Admin.Schema.Constants;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingWerdGestopt
{
    private readonly V041_FeitelijkeVerenigingWerdGestopt _scenario;
    private readonly string _goldenMasterWithOneVereniging;
    private readonly AdminApiClient _adminApiClient;

    private const string EmptyVerenigingenResponse =
        "{\"@context\":\"http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json\",\"verenigingen\": [], \"metadata\": {\"pagination\": {\"totalCount\": 0,\"offset\": 0,\"limit\": 50}}}";

    public Given_VerenigingWerdGestopt(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V041FeitelijkeVerenigingWerdGestopt;
        _adminApiClient = fixture.AdminApiClient;

        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_VerenigingWerdGestopt)}_{nameof(Then_one_vereniging_is_retrieved_by_its_vCode)}");
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
                                   .WithStatus(VerenigingStatus.Gestopt));

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
