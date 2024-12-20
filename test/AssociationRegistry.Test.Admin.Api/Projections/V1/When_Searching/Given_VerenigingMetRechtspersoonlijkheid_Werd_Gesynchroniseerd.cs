namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Searching;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging;
using Common.Extensions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;
using VerenigingStatus = AssociationRegistry.Admin.Schema.Constants.VerenigingStatus;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheid_Werd_Gesynchroniseerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced _scenario;

    public Given_VerenigingMetRechtspersoonlijkheid_Werd_Gesynchroniseerd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V062VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndSynced;
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
                               v => v
                                   .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                                   .WithType(Verenigingstype.Parse(_scenario.RechtsvormWerdGewijzigdInKBO.Rechtsvorm))
                                   .WithNaam(_scenario.NaamWerdGewijzigdInKbo.Naam)
                                   .WithKorteNaam(_scenario.KorteNaamWerdGewijzigdInKbo.KorteNaam)
                                   .WithLocatie(_scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Locatietype,
                                                _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Naam,
                                                _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.ToAdresString(),
                                                _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Postcode,
                                                _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.Adres.Gemeente,
                                                _scenario.VCode,
                                                _scenario.MaatschappelijkeZetelWerdGewijzigdInKbo.Locatie.LocatieId)
                                   .WithEinddatum(_scenario.VerenigingWerdGestoptInKbo.Einddatum)
                                   .WithStatus(VerenigingStatus.Gestopt)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
