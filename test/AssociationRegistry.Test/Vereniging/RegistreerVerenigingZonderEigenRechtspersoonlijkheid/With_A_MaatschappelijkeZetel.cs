namespace AssociationRegistry.Test.Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Framework;
using AutoFixture;
using Common.Stubs.VCodeServices;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Xunit;

public class With_A_MaatschappelijkeZetel
{
    [Fact]
    public async ValueTask Then_Throws_MaatschappelijkeZetelIsNietToegestaan()
    {
        var fixture = new Fixture().CustomizeDomain();

        var registratieData = new RegistratieDataVerenigingZonderEigenRechtspersoonlijkheid(
            fixture.Create<VerenigingsNaam>(),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Contactgegevens: [],
            Locaties: [fixture.Create<Locatie>() with { Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo }],
            Vertegenwoordigers: [],
            HoofdactiviteitenVerenigingsloket: [],
            Werkingsgebieden: [],
            Bankrekeningnummers: []
        );

        await Assert.ThrowsAsync<MaatschappelijkeZetelIsNietToegestaan>(() =>
            Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
                registratieData,
                potentialDuplicatesSkipped: false,
                bevestigingsToken: string.Empty,
                new StubVCodeService(VCode.Create(1001)),
                new ClockStub(DateTime.Today)
            )
        );
    }
}
