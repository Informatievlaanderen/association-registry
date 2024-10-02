namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching;

using AssociationRegistry.Formats;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_MaatschappelijkeZetelWerdOvergenomenUitKbo
{
    private readonly V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_MaatschappelijkeZetelWerdOvergenomenUitKbo(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V029VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllData;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_with_A_Maatschappelijke_Zetel()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                                   .WithLocatie(_scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Locatietype,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Naam,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres.ToAdresString(),
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres?.Postcode,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres?.Gemeente,
                                                _scenario.VCode,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
                                                _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.IsPrimair
                                    )
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
