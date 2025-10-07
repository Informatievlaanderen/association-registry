namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using global::AutoFixture;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009011");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO1;
    public readonly VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO2;
    public readonly VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO3;
    public readonly VertegenwoordigerWerdToegevoegdVanuitKBO VertegenwoordigerWerdToegevoegdVanuitKBO4;
    public readonly VertegenwoordigerWerdVerwijderdUitKBO VertegenwoordigerWerdVerwijderdVanuitKBO4;
    public readonly int AmountOfVertegenwoordigers = 3;
    private IEvent[] _events;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        VertegenwoordigerWerdToegevoegdVanuitKBO1 = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>() with
        {
            VertegenwoordigerId = 1,
        };

        VertegenwoordigerWerdToegevoegdVanuitKBO2 = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>() with
        {
            VertegenwoordigerId = 2,
        };

        VertegenwoordigerWerdToegevoegdVanuitKBO3 = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>() with
        {
            VertegenwoordigerId = 3,
        };

        // add this one, so we can test nextId
        VertegenwoordigerWerdToegevoegdVanuitKBO4 = fixture.Create<VertegenwoordigerWerdToegevoegdVanuitKBO>() with
        {
            VertegenwoordigerId = 4,
        };

        VertegenwoordigerWerdVerwijderdVanuitKBO4 = new VertegenwoordigerWerdVerwijderdUitKBO(
            4,
            VertegenwoordigerWerdToegevoegdVanuitKBO4.Insz,
            VertegenwoordigerWerdToegevoegdVanuitKBO4.Voornaam,
            VertegenwoordigerWerdToegevoegdVanuitKBO4.Achternaam);

        _events =
        [
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegdVanuitKBO1,
            VertegenwoordigerWerdToegevoegdVanuitKBO2,
            VertegenwoordigerWerdToegevoegdVanuitKBO3,
            VertegenwoordigerWerdToegevoegdVanuitKBO4,
            VertegenwoordigerWerdVerwijderdVanuitKBO4,
        ];
    }

    public override IEnumerable<IEvent> Events()
        => _events;
}
