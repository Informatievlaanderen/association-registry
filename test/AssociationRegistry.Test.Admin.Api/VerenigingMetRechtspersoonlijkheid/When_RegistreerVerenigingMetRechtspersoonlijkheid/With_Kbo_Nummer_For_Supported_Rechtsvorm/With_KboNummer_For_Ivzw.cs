namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using AssociationRegistry.Magda;
using Events;
using Fixtures;
using FluentAssertions;
using FluentAssertions.Execution;
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
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
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
    }
}
