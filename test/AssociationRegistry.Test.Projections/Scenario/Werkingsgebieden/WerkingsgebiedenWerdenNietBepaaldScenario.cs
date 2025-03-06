namespace AssociationRegistry.Test.Projections.Scenario.Werkingsgebieden;

using Events;
using System.Linq;

public class WerkingsgebiedenWerdenNietBepaaldScenario : ScenarioBase
{
    private readonly WerkingsgebiedenWerdenBepaaldScenario _werdBepaaldScenario = new();
    public WerkingsgebiedenWerdenNietBepaald WerkingsgebiedenWerdenNietBepaald { get; }

    public WerkingsgebiedenWerdenNietBepaaldScenario()
    {
        WerkingsgebiedenWerdenNietBepaald = new WerkingsgebiedenWerdenNietBepaald(_werdBepaaldScenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override string VCode => _werdBepaaldScenario.VCode;

    public override EventsPerVCode[] Events => _werdBepaaldScenario.Events.Union(
    [
        new EventsPerVCode(VCode, WerkingsgebiedenWerdenNietBepaald),
    ]).ToArray();
}
