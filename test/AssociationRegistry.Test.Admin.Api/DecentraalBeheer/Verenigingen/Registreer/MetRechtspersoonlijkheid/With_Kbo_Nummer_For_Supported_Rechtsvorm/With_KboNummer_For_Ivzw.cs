namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.
    With_Kbo_Nummer_For_Supported_Rechtsvorm;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

public class RegistreerIVzwSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerIVzwSetup(EventsInDbScenariosFixture fixture) : base(fixture, kboNummer: "0824992720")
    {
    }
}

public class With_KboNummer_For_Ivzw : With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerIVzwSetup>
{
    public With_KboNummer_For_Ivzw(
        EventsInDbScenariosFixture fixture,
        RegistreerIVzwSetup registreerVerenigingMetRechtspersoonlijkheidSetup)
        : base(fixture, registreerVerenigingMetRechtspersoonlijkheidSetup)
    {
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _fixture.DocumentStore
                                          .LightweightSession();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = session
                                                                 .Events
                                                                 .QueryRawEventDataOnly<
                                                                      VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                                                 .Should().ContainSingle(
                                                                      e => e.KboNummer == RegistreerVerenigingMetRechtspersoonlijkheidSetup
                                                                                         .UitKboRequest.KboNummer).Subject;

        using (new AssertionScope())
        {
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam.Should().Be("Kometsoft");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam.Should().Be("V.L.K.");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Should().Be(new DateOnly(year: 1989, month: 10, day: 03));
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm.Should().Be(Verenigingstype.IVZW.Code);
        }

        var fetchStreamAsync = await session.Events.FetchStreamAsync(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

        var maatschappelijkeZetelWerdOvergenomenUitKbo = fetchStreamAsync
                                                        .Should().ContainSingle(
                                                             e => e.Data.GetType() == typeof(MaatschappelijkeZetelWerdOvergenomenUitKbo))
                                                        .Subject;

        maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Should().BeEquivalentTo(
            new MaatschappelijkeZetelWerdOvergenomenUitKbo(
                new Registratiedata.Locatie(
                    LocatieId: 1,
                    Locatietype.MaatschappelijkeZetelVolgensKbo,
                    IsPrimair: false,
                    string.Empty,
                    new Registratiedata.Adres(
                        Straatnaam: "Koningsstraat",
                        Huisnummer: "4",
                        string.Empty,
                        Postcode: "1210",
                        Gemeente: "Sint-Joost-ten-Node",
                        Land: "België"),
                    AdresId: null)
            )
        );
    }

    [Fact]
    public async Task Then_It_Adds_Vertegenwoordigers_From_Temporary_Source()
    {
        await using var session = _fixture.DocumentStore
                                          .LightweightSession();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = session
                                                                 .Events
                                                                 .QueryRawEventDataOnly<
                                                                      VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                                                 .Should().ContainSingle(
                                                                      e => e.KboNummer == RegistreerVerenigingMetRechtspersoonlijkheidSetup
                                                                                         .UitKboRequest.KboNummer).Subject;

        var vertegenwoordigers =
            (await session.Events.FetchStreamAsync(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode))
           .Where(e => e.Data.GetType() == typeof(VertegenwoordigerWerdOvergenomenUitKBO))
           .Select(e => e.Data)
           .ToList();

        vertegenwoordigers.Should().BeEquivalentTo(
            new List<VertegenwoordigerWerdOvergenomenUitKBO>
            {
                new(
                    VertegenwoordigerId: 1,
                    Insz: "1234567890",
                    Voornaam: "Ikkeltje",
                    Achternaam: "Persoon"),
                new(
                    VertegenwoordigerId: 2,
                    Insz: "0987654321",
                    Voornaam: "Kramikkeltje",
                    Achternaam: "Persoon"),
            });
    }
}
