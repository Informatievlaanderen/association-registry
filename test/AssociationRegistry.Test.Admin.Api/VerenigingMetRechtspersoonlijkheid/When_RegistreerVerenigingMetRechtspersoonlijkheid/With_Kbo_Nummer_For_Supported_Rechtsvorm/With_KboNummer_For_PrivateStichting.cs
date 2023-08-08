namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using AssociationRegistry.Magda;
using Events;
using Fixtures;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

public class RegistreerPrivateStichtingSetup: RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerPrivateStichtingSetup(EventsInDbScenariosFixture fixture): base(fixture, "0546572531")
    {

    }
}

public class With_KboNummer_For_PrivateStichting: With_KboNummer_For_Supported_Vereniging, IClassFixture<RegistreerPrivateStichtingSetup>
{
    public With_KboNummer_For_PrivateStichting(EventsInDbScenariosFixture fixture, RegistreerPrivateStichtingSetup registreerSetup) : base(fixture, registreerSetup)
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

            verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm.Should().Be(Rechtsvorm.PrivateStichting.Waarde);
        }
    }
}
