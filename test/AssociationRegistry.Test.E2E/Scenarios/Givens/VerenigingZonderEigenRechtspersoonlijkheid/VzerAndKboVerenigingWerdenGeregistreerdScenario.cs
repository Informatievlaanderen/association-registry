namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Enriched;
using EventStore;
using Framework.Mappers;
using Framework.TestClasses;
using MartenDb.Store;
using Vereniging;

public class VzerAndKboVerenigingWerdenGeregistreerdScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario = new();
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    private CommandMetadata Metadata;

    public VzerAndKboVerenigingWerdenGeregistreerdScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        await VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario.GivenEvents(service);
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = fixture.Create<VCode>(),
            Naam = "Vereniging met rechtspersoonlijkheid",
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd]),
            new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd]),
        ];
    }

    public VertegenwoordigerPersoonsgegevensDocument[] GivenVertegenwoordigerPersoonsgegevens()
        => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario.GivenVertegenwoordigerPersoonsgegevens();

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
