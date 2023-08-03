namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Unsupported_Rechtsvorm;

using System.Net;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public abstract class With_KboNummer_For_Unsupported_Vereniging
{
    private readonly EventsInDbScenariosFixture _fixture;
    public readonly RegistreerVereniginMetRechtspersoonlijkheidSetup RegistreerVereniginMetRechtspersoonlijkheidSetup;

    public With_KboNummer_For_Unsupported_Vereniging(EventsInDbScenariosFixture fixture, RegistreerVereniginMetRechtspersoonlijkheidSetup registreerVereniginMetRechtspersoonlijkheidSetup)
    {
        _fixture = fixture;
        RegistreerVereniginMetRechtspersoonlijkheidSetup = registreerVereniginMetRechtspersoonlijkheidSetup;
    }

    [Fact]
    public void Then_it_saves_no_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        session.Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
            .Where(e => e.KboNummer == RegistreerVereniginMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer)
            .Should().BeNullOrEmpty();
    }
}
