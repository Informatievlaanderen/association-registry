namespace AssociationRegistry.Test.Admin.Api.Projections.When_Searching;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Formats;
using Framework;
using Framework.Fixtures;
using Framework.templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel
{
    private readonly V044_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V044VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelVolgensKbo;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vCode_searched()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                                   .WithLocatie(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
                                                _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.Naam,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString(),
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres?.Postcode,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres?.Gemeente,
                                                _scenario.VCode,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
                                                _scenario.MaatschappelijkeZetelVolgensKBOWerdGewijzigd.IsPrimair
                                    ));

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
