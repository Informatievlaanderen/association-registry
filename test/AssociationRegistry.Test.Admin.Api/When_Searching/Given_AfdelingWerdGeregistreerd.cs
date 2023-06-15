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
public class Given_AfdelingWerdGeregistreerd
{
    private readonly string _goldenMasterWithOneVereniging;
    private readonly AdminApiClient _adminApiClient;
    private readonly V019_AfdelingWerdGeregistreerd_WithMinimalFields _scenario;

    public Given_AfdelingWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V019AfdelingWerdGeregistreerdWithMinimalFields;
        _adminApiClient = fixture.AdminApiClient;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_AfdelingWerdGeregistreerd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_vCode_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", _scenario.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
