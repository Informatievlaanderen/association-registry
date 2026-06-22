namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public VerenigingWerdGestopt VerenigingWerdGestopt { get; set; }
    private CommandMetadata Metadata;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = await service.GetNext(),
            };

        VerenigingWerdGestopt = fixture.Create<VerenigingWerdGestopt>();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VerenigingWerdGestopt]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
