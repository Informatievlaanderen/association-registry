namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching;

using Framework.Fixtures;
using Framework.templates;
using Common.Scenarios.EventsInDb;
using Common.Extensions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_BasisGegevenWerdGewijzigd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V004_AlleBasisGegevensWerdenGewijzigd _scenario;

    public Given_BasisGegevenWerdGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V004AlleBasisGegevensWerdenGewijzigd;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v =>
                               {
                                   foreach (var werkingsgebied in _scenario.WerkingsgebiedenWerdenGewijzigd.Werkingsgebieden)
                                   {
                                       v.WithWerkingsgebied(werkingsgebied.Code, werkingsgebied.Naam);
                                   }

                                   return v
                                         .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                                         .WithNaam(_scenario.NaamWerdGewijzigd.Naam)
                                         .WithKorteNaam(_scenario.KorteNaamWerdGewijzigd.KorteNaam)
                                         .WithDoelgroep(_scenario.VCode, _scenario.DoelgroepWerdGewijzigd.Doelgroep.Minimumleeftijd,
                                                        _scenario.DoelgroepWerdGewijzigd.Doelgroep.Maximumleeftijd);

                                   ;
                               });

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
