namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using Framework;
using FluentAssertions;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_BasisGegevenWerdGewijzigd
{
    private readonly string _goldenMasterWithOneVereniging;
    private readonly PublicApiClient _publicApiClient;
    private readonly V003_BasisgegevensWerdenGewijzigdScenario _scenario;
    private string _query = "Oarelbeke Weireldstad";

    public Given_BasisGegevenWerdGewijzigd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V003BasisgegevensWerdenGewijzigdScenario;

        _goldenMasterWithOneVereniging = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_BasisGegevenWerdGewijzigd)}_{nameof(Then_we_retrieve_one_vereniging_matching_the_name_searched)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_query)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(_query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster =
            new ZoekVerenigingenResponseTemplate()
               .FromQuery(_query)
               .WithVereniging()
               .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
               .WithNaam(_scenario.NaamWerdGewijzigd.Naam)
               .WithKorteNaam(_scenario.KorteNaamWerdGewijzigd.KorteNaam)
               .WithDoelgroep(_scenario.DoelgroepWerdGewijzigd.Doelgroep.Minimumleeftijd,
                              _scenario.DoelgroepWerdGewijzigd.Doelgroep.Maximumleeftijd)
               .And()
               .Build();

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
