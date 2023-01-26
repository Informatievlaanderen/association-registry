namespace AssociationRegistry.Test.Public.Api.Given_BasisGegevenWerdGewijzigd;

using AssociationRegistry.Test.Public.Api.Fixtures;
using AssociationRegistry.Test.Public.Api.Framework;
using Fixtures.GivenEvents;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class When_Searching_By_Name
{
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;
    private readonly KorteBeschrijvingWerdGewijzigdScenario _scenario;

    public When_Searching_By_Name(GivenEventsFixture fixture)
    {
        _scenario = fixture.KorteBeschrijvingWerdGewijzigdScenario;
        _publicApiClient = fixture.PublicApiClient;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"{nameof(When_Searching_By_Name)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.Naam)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(_scenario.Naam);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
            .Replace("{{originalQuery}}", _scenario.Naam);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
