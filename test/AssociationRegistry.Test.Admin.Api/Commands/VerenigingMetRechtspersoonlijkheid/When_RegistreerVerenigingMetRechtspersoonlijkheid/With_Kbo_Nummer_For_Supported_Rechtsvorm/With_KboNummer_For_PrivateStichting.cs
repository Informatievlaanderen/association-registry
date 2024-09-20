namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    With_Kbo_Nummer_For_Supported_Rechtsvorm;

using Events;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework.Categories;
using Framework.Fixtures;
using Vereniging;
using Xunit;
using Xunit.Categories;

public class RegistreerPrivateStichtingSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerPrivateStichtingSetup(EventsInDbScenariosFixture fixture) : base(fixture, kboNummer: "0546572531")
    {
    }
}
[Category("AdminApi")]
[IntegrationTestToRefactor]
public class With_KboNummer_For_PrivateStichting : With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerPrivateStichtingSetup>
{
    public With_KboNummer_For_PrivateStichting(EventsInDbScenariosFixture fixture, RegistreerPrivateStichtingSetup registreerSetup) : base(
        fixture, registreerSetup)
    {
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
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
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam.Should().Be("Kom op tegen Kanker");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam.Should().Be("V.L.K.");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Should().Be(new DateOnly(year: 1989, month: 10, day: 03));

            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm.Should().Be(Verenigingstype.PrivateStichting.Code);
        }
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
