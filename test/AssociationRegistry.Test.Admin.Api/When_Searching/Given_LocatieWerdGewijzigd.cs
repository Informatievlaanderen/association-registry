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
public class Given_LocatieWerdGewijzigd
{
    private readonly V027_LocatieWerdGewijzigd _scenario;
    private readonly string _goldenMaster;
    private readonly AdminApiClient _adminApiClient;

    public Given_LocatieWerdGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V027LocatieWerdGewijzigd;
        _adminApiClient = fixture.AdminApiClient;
        _goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_LocatieWerdGewijzigd)}_{nameof(Then_we_retrieve_one_vereniging_with_the_changed_Locatie)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();


    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_with_the_changed_Locatie()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMaster
            .Replace("{{originalQuery}}", _scenario.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
