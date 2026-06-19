namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.MetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.With_Kbo_Nummer_For_Unsupported_Organisaties;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Marten;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public abstract class With_KboNummer_For_Unsupported_Organisatie
{
    private readonly EventsInDbScenariosFixture _fixture;
    public readonly RegistreerVereniginMetRechtspersoonlijkheidSetup RegistreerVereniginMetRechtspersoonlijkheidSetup;

    public With_KboNummer_For_Unsupported_Organisatie(
        EventsInDbScenariosFixture fixture,
        RegistreerVereniginMetRechtspersoonlijkheidSetup registreerVereniginMetRechtspersoonlijkheidSetup
    )
    {
        _fixture = fixture;
        RegistreerVereniginMetRechtspersoonlijkheidSetup = registreerVereniginMetRechtspersoonlijkheidSetup;
    }

    [Fact]
    public async ValueTask Then_it_saves_no_events()
    {
        await using var session = _fixture.DocumentStore.LightweightSession();

        var events = await session
            .Events.QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
               .Where(e => e.KboNummer == RegistreerVereniginMetRechtspersoonlijkheidSetup.UitKboRequest.KboNummer)
            .ToListAsync();

        events.Should().BeNullOrEmpty();
    }
}
