namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    With_Kbo_Nummer_For_Unsupported_Organisaties;

using Events;
using FluentAssertions;
using Framework.Fixtures;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public abstract class With_KboNummer_For_Unsupported_Organisatie
{
    private readonly EventsInDbScenariosFixture _fixture;
    public readonly RegistreerVereniginMetRechtspersoonlijkheidSetup RegistreerVereniginMetRechtspersoonlijkheidSetup;

    public With_KboNummer_For_Unsupported_Organisatie(
        EventsInDbScenariosFixture fixture,
        RegistreerVereniginMetRechtspersoonlijkheidSetup registreerVereniginMetRechtspersoonlijkheidSetup)
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
