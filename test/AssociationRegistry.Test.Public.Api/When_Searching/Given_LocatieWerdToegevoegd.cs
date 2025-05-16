namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formats;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_LocatieWerdToegevoegd
{
    private readonly V011_LocatieWerdToegevoegdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_LocatieWerdToegevoegd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V011LocatieWerdToegevoegdScenario;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async ValueTask? Then_we_retrieve_one_vereniging_matching_the_vcode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                                   .WithLocatie(_scenario.LocatieWerdToegevoegd.Locatie.Locatietype,
                                                _scenario.LocatieWerdToegevoegd.Locatie.Naam,
                                                _scenario.LocatieWerdToegevoegd.Locatie.Adres?.ToAdresString(),
                                                _scenario.LocatieWerdToegevoegd.Locatie.Adres?.Postcode,
                                                _scenario.LocatieWerdToegevoegd.Locatie.Adres?.Gemeente,
                                                _scenario.VCode,
                                                _scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
                                                _scenario.LocatieWerdToegevoegd.Locatie.IsPrimair)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
