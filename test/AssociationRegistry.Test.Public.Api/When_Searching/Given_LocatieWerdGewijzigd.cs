namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Common.Extensions;
using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formats;
using templates;
using Vereniging;

using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_LocatieWerdGewijzigd
{
    private readonly V013_LocatieWerdGewijzigdScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_LocatieWerdGewijzigd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V013LocatieWerdGewijzigdScenario;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_with_the_changed_Locatie()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .WithVCode(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                                   .WithType(Verenigingstype.FeitelijkeVereniging)
                                   .WithNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam)
                                   .WithDoelgroep(_scenario.VCode)
                                   .WithLocatie(_scenario.LocatieWerdGewijzigd.Locatie.Locatietype,
                                                _scenario.LocatieWerdGewijzigd.Locatie.Naam,
                                                _scenario.LocatieWerdGewijzigd.Locatie.Adres?.ToAdresString(),
                                                _scenario.LocatieWerdGewijzigd.Locatie.Adres?.Postcode,
                                                _scenario.LocatieWerdGewijzigd.Locatie.Adres?.Gemeente,
                                                _scenario.VCode,
                                                _scenario.LocatieWerdGewijzigd.Locatie.LocatieId,
                                                _scenario.LocatieWerdGewijzigd.Locatie.IsPrimair)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
