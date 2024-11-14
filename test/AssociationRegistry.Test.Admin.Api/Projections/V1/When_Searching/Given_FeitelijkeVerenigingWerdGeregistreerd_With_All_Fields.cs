namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields
{
    private readonly V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_FeitelijkeVerenigingWerdGeregistreerd_With_All_Fields(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var query = "Feestcommittee Tralalala";
        var response = await _adminApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(query)
                          .WithVereniging(
                               v => v
                                  .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_name()
    {
        var response = await _adminApiClient.Search("stcommittee");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_part_of_its_name_when_using_wildcards()
    {
        var query = "*stcommitte*";
        var response = await _adminApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(query)
                          .WithVereniging(
                               v => v
                                  .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_full_term_within_its_name()
    {
        var query = "Feestcommittee";
        var response = await _adminApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(query)
                          .WithVereniging(
                               v => v
                                  .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_retrieved_by_its_vCode()
    {
        var query = _scenario.VCode;
        var response = await _adminApiClient.Search(query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(query)
                          .WithVereniging(
                               v => v
                                  .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task? Then_one_vereniging_is_not_retrieved_by_part_of_its_vCode()
    {
        var response = await _adminApiClient.Search("88888");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().BeEquivalentJson(new ZoekVerenigingenResponseTemplate());
    }
}
