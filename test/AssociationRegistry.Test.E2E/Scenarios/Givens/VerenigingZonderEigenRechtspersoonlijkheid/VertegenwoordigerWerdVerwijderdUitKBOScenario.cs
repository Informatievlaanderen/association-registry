namespace AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using Framework.TestClasses;

public class VertegenwoordigerWerdVerwijderdUitKBOScenario : IScenario
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO { get; set; }
    public VertegenwoordigerWerdVerwijderdUitKBO VertegenwoordigerWerdVerwijderdUitKBO { get; set; }

    private CommandMetadata Metadata;

    public VertegenwoordigerWerdVerwijderdUitKBOScenario()
    {
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = await service.GetNext(),
        };

        VertegenwoordigerWerdToegevoegdVanuitKBO = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>();
        VertegenwoordigerWerdVerwijderdUitKBO = fixture.Create<VertegenwoordigerWerdVerwijderdUitKBO>() with // values would be the same as the toegevoegde vertegenwoordiger
        {
            VertegenwoordigerId = VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId,
            Insz = VertegenwoordigerWerdToegevoegdVanuitKBO.Insz,
            Voornaam = VertegenwoordigerWerdToegevoegdVanuitKBO.Voornaam,
            Achternaam = VertegenwoordigerWerdToegevoegdVanuitKBO.Achternaam,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        return
        [
            new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, [VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdToegevoegdVanuitKBO, VertegenwoordigerWerdVerwijderdUitKBO]),
        ];
    }

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
