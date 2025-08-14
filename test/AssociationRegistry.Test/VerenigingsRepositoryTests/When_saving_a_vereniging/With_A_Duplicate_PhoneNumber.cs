﻿namespace AssociationRegistry.Test.VerenigingsRepositoryTests.When_saving_a_vereniging;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using DecentraalBeheer.Vereniging.Geotags;
using Moq;
using Xunit;

public class With_A_Duplicate_PhoneNumber
{
    [Theory]
    [InlineData("+32 412 34 56 78", "0032412345678")]
    [InlineData("+32 051 12 34 56", "003251123456")]
    [InlineData("+32 51 12 34 56", "003251123456")]
    [InlineData("+32 (412)/34.56.78", "0032412345678")]
    [InlineData("(+32) 0412 34 56 78", "0032412345678")]
    [InlineData("+32 412/34-56-78", "0032412345678")]
    [InlineData("+32412345678", "0032412345678")]
    [InlineData("+32 412 34 56 78 (home)", "0032412345678")]
    [InlineData("0412 34 56 78", "0032412345678")]
    [InlineData("0412345678", "0032412345678")]
    [InlineData("051 12 34 56", "003251123456")]
    [InlineData("051123456", "003251123456")]
    [InlineData("0032 412 34 56 78", "0032412345678")]
    [InlineData("+31 (0)20 369 0664", "0031203690664")]
    [InlineData("+4989 9982804-50", "004989998280450")]
    [InlineData("0032412345678", "+32 412 34 56 78")]
    [InlineData("003251123456", "+32 051 12 34 56")]
    [InlineData("003251123456", "+32 51 12 34 56")]
    [InlineData("0032412345678", "+32 (412)/34.56.78")]
    [InlineData("0032412345678", "(+32) 0412 34 56 78")]
    [InlineData("0032412345678", "+32 412/34-56-78")]
    [InlineData("0032412345678", "+32412345678")]
    [InlineData("0032412345678", "+32 412 34 56 78 (home)")]
    [InlineData("0032412345678", "0412 34 56 78")]
    [InlineData("0032412345678", "0412345678")]
    [InlineData("003251123456", "051 12 34 56")]
    [InlineData("003251123456", "051123456")]
    [InlineData("0032412345678", "0032 412 34 56 78")]
    [InlineData("0031203690664", "+31 (0)20 369 0664")]
    [InlineData("004989998280450", "+4989 9982804-50")]
    public async ValueTask Then_Throws_ContactgegevenIsDuplicaatExceptions(string waarde, string otherWaarde)
    {
        var fixture = new Fixture().CustomizeDomain();
        var vCode = VCode.Create(1001);

        var vCodeService = new StubVCodeService(vCode);

        var command = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
            Naam: fixture.Create<VerenigingsNaam>(),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep: Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Contactgegevens: CreateDuplicateContactgegevens(waarde, fixture),
            Locaties: [],
            Vertegenwoordigers: [],
            HoofdactiviteitenVerenigingsloket: [],
            Werkingsgebieden: []);

        await Assert.ThrowsAsync<ContactgegevenIsDuplicaat>(() => Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
                                                                command,
                                                                vCodeService,
                                                                Mock.Of<IGeotagsService>(),
                                                                clock: new ClockStub(DateTime.Today)));
    }

    private static Contactgegeven[] CreateDuplicateContactgegevens(string waarde, Fixture fixture)
    {
        var beschrijving = "zelfde beschrijving";

        return
        [
            fixture.Create<Contactgegeven>() with
            {
                Beschrijving = beschrijving,
                Contactgegeventype = Contactgegeventype.Telefoon,
                Waarde = waarde,
            },
            fixture.Create<Contactgegeven>() with
            {
                Beschrijving = beschrijving,
                Contactgegeventype = Contactgegeventype.Telefoon,
                Waarde = waarde,
            },
        ];
    }
}
