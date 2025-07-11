﻿namespace AssociationRegistry.Test.When_RegistreerVerenigingZonderEigenRechtspersoonlijkheid.Geotags;

using AutoFixture;
using Common.AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.Faktories;
using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using EventFactories;
using Events;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Vereniging.Geotags;
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
        var vereniging = await Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(registreerVerenigingZonderEigenRechtspersoonlijkheidCommand, new StubVCodeService(vCode), geotagsService.Object, clock);
        await vereniging.BerekenGeotags(geotagsService.Object);

        vereniging.UncommittedEvents.OfType<GeotagsWerdenBepaald>().Single()
                  .ShouldCompare(EventFactory.GeotagsWerdenBepaald(vCode, GeotagsCollection.Empty));
    }
}
