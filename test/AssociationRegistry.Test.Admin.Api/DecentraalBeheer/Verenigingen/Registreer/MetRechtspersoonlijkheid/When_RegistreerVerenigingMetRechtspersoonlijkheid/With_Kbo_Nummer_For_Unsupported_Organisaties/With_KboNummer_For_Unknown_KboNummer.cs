namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    With_Kbo_Nummer_For_Unsupported_Organisaties;

using FluentAssertions;
using Framework.Fixtures;
using System.Net;
using Xunit;

public class RegistreerOnbekendKboNummerSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerOnbekendKboNummerSetup(EventsInDbScenariosFixture fixture) : base(fixture, kboNummer: "0000000196")
    {
    }
}

public class With_KboNummer_For_Vestiging : With_KboNummer_For_Unsupported_Organisatie, IClassFixture<RegistreerVestigingSetup>
{
    public With_KboNummer_For_Vestiging(EventsInDbScenariosFixture fixture, RegistreerVestigingSetup registreerSetup) : base(
        fixture, registreerSetup)
    {
    }

    [Fact]
    public void Then_it_returns_an_bad_request_response_with_correct_headers()
    {
        RegistreerVereniginMetRechtspersoonlijkheidSetup.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
