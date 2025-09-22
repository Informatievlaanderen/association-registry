namespace AssociationRegistry.Test.When_RegistreerVerenigingZonderEigenRechtspersoonlijkheid.Geotags;

using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Common.AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.Faktories;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Geotags;
using Events;
using Events.Factories;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;

public class Given_Geotags_Returns_Empty_Collection
{
    [Fact]
    public async Task Then_One_GeotagsWerdenBepaaldEvent()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var geotagsService = Faktory.New().GeotagsService.ReturnsEmptyGeotags();
        var clock = Faktory.New().Clock.Stub(DateOnly.FromDateTime(DateTime.Today.AddDays(-5)));

        var vCode = fixture.Create<VCode>();
        var registreerVerenigingZonderEigenRechtspersoonlijkheidCommand = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Startdatum = Datum.Create(clock.Today)
        };
        var registratieData = new RegistratieDataVerenigingZonderEigenRechtspersoonlijkheid(
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.Naam,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.KorteNaam,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.KorteBeschrijving,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.Startdatum,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.Doelgroep,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.IsUitgeschrevenUitPubliekeDatastroom,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.Contactgegevens,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.Locaties,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.Vertegenwoordigers,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.HoofdactiviteitenVerenigingsloket,
            registreerVerenigingZonderEigenRechtspersoonlijkheidCommand.Werkingsgebieden);

        var vereniging = await Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(registratieData,
            false,
            string.Empty, new StubVCodeService(vCode),
            clock);

        await vereniging.BerekenGeotags(geotagsService.Object);

        vereniging.UncommittedEvents.OfType<GeotagsWerdenBepaald>().Single()
                  .ShouldCompare(EventFactory.GeotagsWerdenBepaald(vCode, GeotagsCollection.Empty));
    }
}
