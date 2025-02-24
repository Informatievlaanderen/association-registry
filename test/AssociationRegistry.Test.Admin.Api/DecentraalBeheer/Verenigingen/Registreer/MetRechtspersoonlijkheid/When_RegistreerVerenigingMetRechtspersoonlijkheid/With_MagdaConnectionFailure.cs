namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using FluentAssertions;
using Framework.Fixtures;
using System.Net;
using With_Kbo_Nummer_For_Unsupported_Organisaties;
using Xunit;

public class ConnectionFailureSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    private const string KboNummerForConnectionFailureInWiremock = "0132575046";

    public ConnectionFailureSetup(EventsInDbScenariosFixture fixture) : base(fixture, KboNummerForConnectionFailureInWiremock)
    {
    }
}

public class With_MagdaConnectionFailure : With_KboNummer_For_Unsupported_Organisatie, IClassFixture<ConnectionFailureSetup>
{
    public With_MagdaConnectionFailure(EventsInDbScenariosFixture fixture, ConnectionFailureSetup registreerSetup) : base(
        fixture, registreerSetup)
    {
    }

    [Fact]
    public void Then_it_returns_an_internal_server_error_response()
    {
        RegistreerVereniginMetRechtspersoonlijkheidSetup.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
