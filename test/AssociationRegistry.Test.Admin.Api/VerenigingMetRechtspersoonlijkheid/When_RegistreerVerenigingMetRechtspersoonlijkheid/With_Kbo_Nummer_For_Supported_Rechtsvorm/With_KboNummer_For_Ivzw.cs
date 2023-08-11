namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using AssociationRegistry.Magda;
using Events;
using Fixtures;
using FluentAssertions;
using FluentAssertions.Execution;
using Vereniging;
using Xunit;

public class RegistreerIVzwSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerIVzwSetup(EventsInDbScenariosFixture fixture) : base(fixture, "0824992720")
    {
    }
}

public class With_KboNummer_For_Ivzw : With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerIVzwSetup>
{
    public With_KboNummer_For_Ivzw(EventsInDbScenariosFixture fixture, RegistreerIVzwSetup registreerVereniginMetRechtspersoonlijkheidSetup) : base(fixture, registreerVereniginMetRechtspersoonlijkheidSetup)
    {
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _fixture.DocumentStore
            .LightweightSession();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Should().ContainSingle(e => e.KboNummer == _registreerVereniginMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer).Subject;

        using (new AssertionScope())
        {
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam.Should().Be("Kometsoft");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam.Should().Be("V.L.K.");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Should().Be(new DateOnly(1989, 10, 03));
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm.Should().Be(Rechtsvorm.IVZW.Waarde);
        }

        var fetchStreamAsync = await session.Events.FetchStreamAsync(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fetchStreamAsync
            .Should().ContainSingle(e => e.Data.GetType() == typeof(MaatschappelijkeZetelWerdOvergenomenUitKbo)).Subject;

        maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Should().BeEquivalentTo(
            new MaatschappelijkeZetelWerdOvergenomenUitKbo(
                new Registratiedata.Locatie(
                    1,
                    Locatietype.MaatschappelijkeZetelVolgensKbo,
                    false,
                    string.Empty,
                    new Registratiedata.Adres(
                        "Koningsstraat",
                        "4",
                        string.Empty,
                        "1210",
                        "Sint-Joost-ten-Node",
                        "België"),
                    null)
            )
        );
    }
}
