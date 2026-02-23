namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;
using MartenDb.Store;
using Vereniging;

public class VzerWerdGeregistreerdAfterKszSyncScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    private CommandMetadata Metadata;

    public VzerWerdGeregistreerdAfterKszSyncScenario() { }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = await service.GetNext(),
            };

        var kszSyncHeeftVertegenwoordigerBevestigd = fixture.Create<KszSyncHeeftVertegenwoordigerBevestigd>() with
        {
            VertegenwoordigerId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                .Vertegenwoordigers.First()
                .VertegenwoordigerId,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, kszSyncHeeftVertegenwoordigerBevestigd]
            ),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata() => Metadata;
}
