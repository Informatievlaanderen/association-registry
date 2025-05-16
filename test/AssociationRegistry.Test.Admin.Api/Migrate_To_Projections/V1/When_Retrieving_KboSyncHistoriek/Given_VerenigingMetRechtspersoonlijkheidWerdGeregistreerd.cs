namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_KboSyncHistoriek;

using AssociationRegistry.Admin.Api.Verenigingen.KboSync.ResponseModels;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates.kboSyncHistoriek;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

[Collection(nameof(AdminApiCollection))]
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
    public async ValueTask Then_we_get_registreer_inschrijving_gebeurtenis()
    {
        var content = await _response.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<KboSyncHistoriekResponse>(content);

        List<KboSyncHistoriekGebeurtenisResponse> expected =
        [
            new KboSyncHistoriekGebeurtenisResponse()
            {
                VCode = _inschrijvingZonderSync.VCode,
                Beschrijving = "Registreer inschrijving geslaagd",
                Kbonummer = _inschrijvingZonderSync.KboNummer,
                Tijdstip = _inschrijvingZonderSync.GetCommandMetadata().Tijdstip.FormatAsZuluTime(),
            },new KboSyncHistoriekGebeurtenisResponse()
            {
                VCode = _inschrijvingMetSync.VCode,
                Beschrijving = "Registreer inschrijving geslaagd",
                Kbonummer = _inschrijvingMetSync.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer,
                Tijdstip = _inschrijvingMetSync.GetCommandMetadata().Tijdstip.FormatAsZuluTime(),
            },new KboSyncHistoriekGebeurtenisResponse()
            {
                VCode = _inschrijvingMetSync.VCode,
                Beschrijving = "Vereniging succesvol up to date gebracht met data uit de KBO",
                Kbonummer = _inschrijvingMetSync.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer,
                Tijdstip = _inschrijvingMetSync.GetCommandMetadata().Tijdstip.FormatAsZuluTime(),
            },
        ];

        expected.ForEach(x => response.Should().Contain(x));
    }
}
