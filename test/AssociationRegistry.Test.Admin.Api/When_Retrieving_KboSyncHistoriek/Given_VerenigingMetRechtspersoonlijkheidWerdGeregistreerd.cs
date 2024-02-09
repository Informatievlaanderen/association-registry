namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_KboSyncHistoriek;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using templates.kboSyncHistoriek;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly HttpResponseMessage _response;
    private readonly V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V029VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllData;
        _response = fixture.SuperAdminApiClient.GetKboSyncHistoriek().GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_registreer_inschrijving_gebeurtenis()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected = new KboSyncHistoriekTemplate(
                new KboSyncHistoriekGebeurtenis(
                    _scenario.KboNummer,
                    _scenario.VCode,
                    Beschrijving: "Registreer inschrijving geslaagd",
                    _scenario.GetCommandMetadata().Tijdstip.ToZuluTime()))
           .Build();

        content.Should().BeEquivalentJson(expected);
    }
}
