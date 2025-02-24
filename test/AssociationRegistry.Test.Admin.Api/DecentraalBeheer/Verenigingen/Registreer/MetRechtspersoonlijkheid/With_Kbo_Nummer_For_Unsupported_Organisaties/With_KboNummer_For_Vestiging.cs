namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.
    With_Kbo_Nummer_For_Unsupported_Organisaties;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using System.Net;
using Xunit;

public class RegistreerVestigingSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    public RegistreerVestigingSetup(EventsInDbScenariosFixture fixture) : base(fixture, kboNummer: "2289324120")
    {
    }
}

public class With_KboNummer_For_Unknown_KboNummer : With_KboNummer_For_Unsupported_Organisatie,
                                                    IClassFixture<RegistreerOnbekendKboNummerSetup>
{
    public With_KboNummer_For_Unknown_KboNummer(EventsInDbScenariosFixture fixture, RegistreerOnbekendKboNummerSetup registreerSetup) :
        base(fixture, registreerSetup)
    {
    }

    [Fact]
    public void Then_it_returns_an_bad_request_response_with_correct_headers()
    {
        RegistreerVereniginMetRechtspersoonlijkheidSetup.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
