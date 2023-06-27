namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Framework;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_LocatieWerdToegevoegd
{
    private readonly string _goldenMaster;
    private readonly V011_LocatieWerdToegevoegdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_LocatieWerdToegevoegd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V011LocatieWerdToegevoegdScenario;
        _goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_LocatieWerdToegevoegd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_vcode_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();


    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vcode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMaster
            .Replace("{{originalQuery}}", _scenario.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
