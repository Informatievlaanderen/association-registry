namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using Framework.TestClasses;
using MartenDb.Store;
using Vereniging;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    private CommandMetadata Metadata;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
