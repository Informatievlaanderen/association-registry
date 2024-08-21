namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    With_Kbo_Nummer_For_Supported_Rechtsvorm;

using Events;
using Vereniging;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework.Fixtures;
using Xunit;

public class RegistreerStichtingVanOpenbaarNutSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerStichtingVanOpenbaarNutSetup(EventsInDbScenariosFixture fixture) : base(fixture, kboNummer: "0468831484")
    {
    }
}

public class With_KboNummer_For_StichtingVanOpenbaarNut : With_KboNummer_For_Supported_Vereniging,
                                                          IClassFixture<RegistreerStichtingVanOpenbaarNutSetup>
{
    public With_KboNummer_For_StichtingVanOpenbaarNut(
        EventsInDbScenariosFixture fixture,
        RegistreerStichtingVanOpenbaarNutSetup registreerVerenigingMetRechtspersoonlijkheidSetup) : base(
        fixture, registreerVerenigingMetRechtspersoonlijkheidSetup)
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
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm.Should().Be(Verenigingstype.StichtingVanOpenbaarNut.Code);
        }

        var contactgegevensWerdOvergenomenUitKbo = session.Events.FetchStream(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode)
                                                          .Where(e => e.Data.GetType() == typeof(ContactgegevenWerdOvergenomenUitKBO))
                                                          .Select(e => e.Data)
                                                          .ToList();

        contactgegevensWerdOvergenomenUitKbo.Should().HaveCount(4);

        contactgegevensWerdOvergenomenUitKbo.Should().ContainEquivalentOf(
            new ContactgegevenWerdOvergenomenUitKBO(ContactgegevenId: 1, Contactgegeventype.Email.Waarde,
                                                    ContactgegeventypeVolgensKbo.Email, Waarde: "info@opdebosuil.be"));

        contactgegevensWerdOvergenomenUitKbo.Should().ContainEquivalentOf(
            new ContactgegevenWerdOvergenomenUitKBO(ContactgegevenId: 2, Contactgegeventype.Website.Waarde,
                                                    ContactgegeventypeVolgensKbo.Website, Waarde: "https://www.opdebosuil.be"));

        contactgegevensWerdOvergenomenUitKbo.Should().ContainEquivalentOf(
            new ContactgegevenWerdOvergenomenUitKBO(ContactgegevenId: 3, Contactgegeventype.Telefoon.Waarde,
                                                    ContactgegeventypeVolgensKbo.Telefoon, Waarde: "011642985"));

        contactgegevensWerdOvergenomenUitKbo.Should().ContainEquivalentOf(
            new ContactgegevenWerdOvergenomenUitKBO(ContactgegevenId: 4, Contactgegeventype.Telefoon.Waarde,
                                                    ContactgegeventypeVolgensKbo.GSM, Waarde: "0987654321"));
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
