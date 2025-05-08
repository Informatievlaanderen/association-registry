namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_BasisGegevenWerdGewijzigd
{
    private readonly PublicApiClient _publicApiClient;
    private readonly V003_BasisgegevensWerdenGewijzigdScenario _scenario;
    private readonly string _query = "Oarelbeke Weireldstad";

    public Given_BasisGegevenWerdGewijzigd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V003BasisgegevensWerdenGewijzigdScenario;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_query)).Should().BeSuccessful();

    [Fact]
    public async ValueTask? Then_we_retrieve_one_vereniging_matching_the_name_searched()
    {
        var response = await _publicApiClient.Search(_query);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster =
            new ZoekVerenigingenResponseTemplate()
               .FromQuery(_query)
               .WithVereniging(
                    v => v
                        .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                        .WithNaam(_scenario.NaamWerdGewijzigd.Naam)
                        .WithKorteNaam(_scenario.KorteNaamWerdGewijzigd.KorteNaam)
                        .WithKorteBeschrijving(_scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving)
                        .WithDoelgroep(_scenario.VCode, _scenario.DoelgroepWerdGewijzigd.Doelgroep.Minimumleeftijd,
                                       _scenario.DoelgroepWerdGewijzigd.Doelgroep.Maximumleeftijd)
                );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
