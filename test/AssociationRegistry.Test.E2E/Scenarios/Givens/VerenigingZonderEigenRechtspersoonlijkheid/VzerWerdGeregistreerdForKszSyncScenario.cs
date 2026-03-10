namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class VzerWerdGeregistreerdForKszSyncScenario : IScenario
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    private CommandMetadata Metadata;
    public Registratiedata.Vertegenwoordiger Vertegenwoordiger { get; set; }
    public Registratiedata.Vertegenwoordiger VerwijderdeVertegenwoordiger { get; set; }

    public VertegenwoordigerWerdVerwijderd VertegenwoordigerWerdVerwijderd { get; set; }

    public VzerWerdGeregistreerdForKszSyncScenario() { }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        Vertegenwoordiger = fixture.Create<Registratiedata.Vertegenwoordiger>() with
        {
            VertegenwoordigerId = 1,
            Insz = "99010112390",
        };

        VerwijderdeVertegenwoordiger = fixture.Create<Registratiedata.Vertegenwoordiger>() with
        {
            VertegenwoordigerId = 2,
            Insz = "03010112343",
        };

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = await service.GetNext(),
                Vertegenwoordigers = [Vertegenwoordiger, VerwijderdeVertegenwoordiger],
            };

        VertegenwoordigerWerdVerwijderd = fixture.Create<VertegenwoordigerWerdVerwijderd>() with
        {
            VertegenwoordigerId = VerwijderdeVertegenwoordiger.VertegenwoordigerId,
            Insz = VerwijderdeVertegenwoordiger.Insz,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                [VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdVerwijderd]
            ),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata() => Metadata;
}
