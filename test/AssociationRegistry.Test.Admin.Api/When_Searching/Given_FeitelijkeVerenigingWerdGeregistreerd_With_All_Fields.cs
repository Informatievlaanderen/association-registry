namespace AssociationRegistry.Test.Admin.Api.When_Searching;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields
{
    private readonly V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields _scenario;
    private readonly string _goldenMasterWithOneVereniging;
    private readonly AdminApiClient _adminApiClient;

    private const string EmptyVerenigingenResponse =
        "{\"@context\":\"http://127.0.0.1:11003/v1/contexten/zoek-verenigingen-context.json\",\"verenigingen\": [], \"metadata\": {\"pagination\": {\"totalCount\": 0,\"offset\": 0,\"limit\": 50}}}";

    public Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields;
        _adminApiClient = fixture.AdminApiClient;

        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _adminApiClient.Search("Feestcommittee Oudenaarde");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
           .Replace("{{originalQuery}}", "Feestcommittee Oudenaarde");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var response = await _adminApiClient.Search("dena");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var response = await _adminApiClient.Search("*dena*");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
           .Replace("{{originalQuery}}", "*dena*");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var response = await _adminApiClient.Search("oudenaarde");
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
           .Replace("{{originalQuery}}", "oudenaarde");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMasterWithOneVereniging
           .Replace("{{originalQuery}}", _scenario.VCode);

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var response = await _adminApiClient.Search("88888");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(EmptyVerenigingenResponse);
    }
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingWerdGestopt
{
    private readonly V041_FeitelijkeVerenigingWerdGestopt _scenario;
    private readonly string _goldenMasterWithOneVereniging;
    private readonly AdminApiClient _adminApiClient;

    private const string EmptyVerenigingenResponse =
        "{\"@context\":\"http://127.0.0.1:11003/v1/contexten/zoek-verenigingen-context.json\",\"verenigingen\": [], \"metadata\": {\"pagination\": {\"totalCount\": 0,\"offset\": 0,\"limit\": 50}}}";

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

        var goldenMaster = _goldenMasterWithOneVereniging
           .Replace("{{originalQuery}}", _scenario.VCode);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
