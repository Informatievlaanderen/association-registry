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
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegdVanuitKBO1,
            VertegenwoordigerWerdToegevoegdVanuitKBO2,
        };
}
