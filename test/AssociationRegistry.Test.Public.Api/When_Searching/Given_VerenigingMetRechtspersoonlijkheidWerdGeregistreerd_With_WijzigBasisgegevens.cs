namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens
{
    private readonly V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens _scenario;
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens(GivenEventsFixture fixture)
    {
        _scenario = fixture.V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens;
        _publicApiClient = fixture.PublicApiClient;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_vCode_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
           .Replace("{{originalQuery}}", _scenario.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingWerdGestopt
{
    private readonly V016_VerenigingWerdGestopt _scenario;
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;

    public Given_VerenigingWerdGestopt(GivenEventsFixture fixture)
    {
        _scenario = fixture.V016VerenigingWerdGestopt;
        _publicApiClient = fixture.PublicApiClient;
        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_VerenigingWerdGestopt)}_{nameof(Then_we_retrieve_no_vereniging_matching_the_vCode_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_no_vereniging_matching_the_vCode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = _goldenMasterWithOneVereniging
           .Replace("{{originalQuery}}", _scenario.VCode);
        content.Should().BeEquivalentJson(goldenMaster);
    }
}
