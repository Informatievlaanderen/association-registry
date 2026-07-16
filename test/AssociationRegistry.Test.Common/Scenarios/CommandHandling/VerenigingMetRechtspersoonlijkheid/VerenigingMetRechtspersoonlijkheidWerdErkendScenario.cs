namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VerenigingMetRechtspersoonlijkheidWerdErkendScenario : CommandhandlerScenarioBase
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VerenigingWerdErkend VerenigingWerdErkend;

    public VerenigingMetRechtspersoonlijkheidWerdErkendScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        VerenigingWerdErkend = fixture.Create<VerenigingWerdErkend>();
    }

    public override VCode VCode => VCode.Create("V0009004");

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, VerenigingWerdErkend };
}
