namespace AssociationRegistry.Test.Public.Api.When_Searching;

using AssociationRegistry.Test.Framework;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_LocatieWerdGewijzigd
{
    private readonly V013_LocatieWerdGewijzigdScenario _scenario;
    private readonly string _goldenMaster;
    private readonly PublicApiClient _publicApiClient;

    public Given_LocatieWerdGewijzigd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V013LocatieWerdGewijzigdScenario;
        _goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_LocatieWerdGewijzigd)}_{nameof(Then_we_retrieve_one_vereniging_with_the_changed_Locatie)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_with_the_changed_Locatie()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMaster
            .Replace("{{originalQuery}}", _scenario.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
