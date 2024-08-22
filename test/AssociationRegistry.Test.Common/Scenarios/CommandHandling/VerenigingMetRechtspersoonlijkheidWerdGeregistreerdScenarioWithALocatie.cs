namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Kbo;
using Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithALocatieScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public LocatieWerdToegevoegd LocatieWerdToegevoegd;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithALocatieScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        LocatieWerdToegevoegd = fixture.Create<LocatieWerdToegevoegd>();
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            LocatieWerdToegevoegd
        };
}
