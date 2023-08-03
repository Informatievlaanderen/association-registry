namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Supported_Rechtsvorm;

using System.Net;
using Events;
using Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public abstract class With_KboNummer_For_Unsupported_Vereniging
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly RegistreerVereniginMetRechtspersoonlijkheidSetup _registreerVereniginMetRechtspersoonlijkheidSetup;

    public With_KboNummer_For_Unsupported_Vereniging(EventsInDbScenariosFixture fixture, RegistreerVereniginMetRechtspersoonlijkheidSetup registreerVereniginMetRechtspersoonlijkheidSetup)
    {
        _fixture = fixture;
        _registreerVereniginMetRechtspersoonlijkheidSetup = registreerVereniginMetRechtspersoonlijkheidSetup;
    }

    [Fact]
    public void Then_it_saves_no_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Where(e => e.KboNummer == _registreerVereniginMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer)
            .Should().BeNullOrEmpty();
    }

    [Fact]
    public void Then_it_returns_an_bad_request_response_with_correct_headers()
    {
        _registreerVereniginMetRechtspersoonlijkheidSetup.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
