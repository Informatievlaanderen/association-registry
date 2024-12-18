namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_KboSyncHistoriek;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates.kboSyncHistoriek;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using Common.Extensions;
using FluentAssertions;
using Formats;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly HttpResponseMessage _response;
    private readonly V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data _inschrijvingZonderSync;
    private readonly V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced _inschrijvingMetSync;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _inschrijvingZonderSync = fixture.V029VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllData;
        _inschrijvingMetSync = fixture.V062VerenigingMetRechtspersoonlijkheidWerdGeregistreerdAndSynced;
        _response = fixture.SuperAdminApiClient.GetKboSyncHistoriek().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_registreer_inschrijving_gebeurtenis()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected = new KboSyncHistoriekTemplate(
                new KboSyncHistoriekGebeurtenis(
                    _inschrijvingZonderSync.KboNummer,
                    _inschrijvingZonderSync.VCode,
                    Beschrijving: "Registreer inschrijving geslaagd",
                    _inschrijvingZonderSync.GetCommandMetadata().Tijdstip.FormatAsZuluTime()),
                new KboSyncHistoriekGebeurtenis(
                    _inschrijvingMetSync.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer,
                    _inschrijvingMetSync.VCode,
                    Beschrijving: "Registreer inschrijving geslaagd",
                    _inschrijvingMetSync.GetCommandMetadata().Tijdstip.FormatAsZuluTime()),
                new KboSyncHistoriekGebeurtenis(
                    _inschrijvingMetSync.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer,
                    _inschrijvingMetSync.VCode,
                    Beschrijving: "Vereniging succesvol up to date gebracht met data uit de KBO",
                    _inschrijvingMetSync.GetCommandMetadata().Tijdstip.FormatAsZuluTime())
            )
           .Build();

        content.Should().BeEquivalentJson(expected);
    }
}
