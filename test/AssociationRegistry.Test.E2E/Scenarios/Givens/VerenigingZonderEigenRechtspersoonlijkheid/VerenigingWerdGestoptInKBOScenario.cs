namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class VerenigingWerdGestoptInKBOScenario : IScenario
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO1 { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO2 { get; set; }

    public VerenigingWerdGestoptInKBO VerenigingWerdGestoptInKBO { get; set; }

    private CommandMetadata Metadata;

    public VerenigingWerdGestoptInKBOScenario() { }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = await service.GetNext(),
            };

        VerenigingWerdGestoptInKBO = fixture.Create<VerenigingWerdGestoptInKBO>();

        VertegenwoordigerWerdToegevoegdVanuitKBO1 = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>() with
        {
            VertegenwoordigerId = 1,
        };
        VertegenwoordigerWerdToegevoegdVanuitKBO2 = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>() with
        {
            VertegenwoordigerId = 2,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(
                VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                [
                    VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
                    VertegenwoordigerWerdToegevoegdVanuitKBO1,
                    VertegenwoordigerWerdToegevoegdVanuitKBO2,
                    VerenigingWerdGestoptInKBO,
                ]
            ),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata() => Metadata;
}
