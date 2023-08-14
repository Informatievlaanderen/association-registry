namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using Events;
using Fixtures;
using FluentAssertions;
using FluentAssertions.Execution;
using Vereniging;
using Xunit;

public class RegistreerStichtingVanOpenbaarNutSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerStichtingVanOpenbaarNutSetup(EventsInDbScenariosFixture fixture) : base(fixture, "0468831484")
    {
    }
}

public class With_KboNummer_For_StichtingVanOpenbaarNut : With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerStichtingVanOpenbaarNutSetup>
{
    public With_KboNummer_For_StichtingVanOpenbaarNut(EventsInDbScenariosFixture fixture, RegistreerStichtingVanOpenbaarNutSetup registreerVereniginMetRechtspersoonlijkheidSetup) : base(fixture, registreerVereniginMetRechtspersoonlijkheidSetup)
    {
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Should().ContainSingle(e => e.KboNummer == _registreerVereniginMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer).Subject;

        using (new AssertionScope())
        {
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam.Should().Be("Kom op tegen Kanker");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.KorteNaam.Should().Be("V.L.K.");
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum.Should().Be(new DateOnly(1989, 10, 03));
            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm.Should().Be(Verenigingstype.StichtingVanOpenbaarNut.Code);
        }

        var contactgegevensWerdOvergenomenUitKbo = session.Events.FetchStream(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode)
            .Where(e => e.Data.GetType() == typeof(ContactgegevenWerdOvergenomenUitKBO))
            .Select(e => e.Data)
            .ToList();

        contactgegevensWerdOvergenomenUitKbo.Should().HaveCount(3);
        contactgegevensWerdOvergenomenUitKbo.Should().ContainEquivalentOf(new ContactgegevenWerdOvergenomenUitKBO(1, ContactgegevenType.Email.Waarde, "info@opdebosuil.be"));
        contactgegevensWerdOvergenomenUitKbo.Should().ContainEquivalentOf(new ContactgegevenWerdOvergenomenUitKBO(2, ContactgegevenType.Website.Waarde, "https://www.opdebosuil.be"));
        contactgegevensWerdOvergenomenUitKbo.Should().ContainEquivalentOf(new ContactgegevenWerdOvergenomenUitKBO(3, ContactgegevenType.Telefoon.Waarde, "011642985"));
    }
}
