namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using Fixtures;
using FluentAssertions;
using System.Net;
using With_Kbo_Nummer_For_Unsupported_Organisaties;
using Xunit;

public class ConnectionFailureSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public ConnectionFailureSetup(EventsInDbScenariosFixture fixture) : base(fixture, kboNummer: "0898251969")
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
