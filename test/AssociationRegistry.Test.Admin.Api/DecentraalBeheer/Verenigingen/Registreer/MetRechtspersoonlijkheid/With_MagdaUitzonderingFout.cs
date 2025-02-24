namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using System.Net;
using With_Kbo_Nummer_For_Unsupported_Organisaties;
using Xunit;

public class MagdaUitzonderingFoutSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    private const string KboNummerForMagdaUitzonderingFoutInWiremock = "0725459040";

    public MagdaUitzonderingFoutSetup(EventsInDbScenariosFixture fixture) : base(fixture, KboNummerForMagdaUitzonderingFoutInWiremock)
    {
    }
}

public class With_MagdaUitzonderingFout : With_KboNummer_For_Unsupported_Organisatie, IClassFixture<MagdaUitzonderingFoutSetup>
{
    public With_MagdaUitzonderingFout(EventsInDbScenariosFixture fixture, MagdaUitzonderingFoutSetup registreerSetup) : base(
        fixture, registreerSetup)
    {
    }

    [Fact]
    public void Then_it_returns_an_internal_server_error_response()
    {
        RegistreerVereniginMetRechtspersoonlijkheidSetup.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
