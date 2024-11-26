namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using Events;
using Framework;

public class WerkingsgebiedenWerdenNietBepaaldScenario : ScenarioBase
{
    private readonly WerkingsgebiedenWerdenBepaaldScenario _werdBepaaldScenario = new();
    public WerkingsgebiedenWerdenNietBepaald WerkingsgebiedenWerdenNietBepaald { get; }

    public WerkingsgebiedenWerdenNietBepaaldScenario()
    {
        WerkingsgebiedenWerdenNietBepaald = new WerkingsgebiedenWerdenNietBepaald(_werdBepaaldScenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override EventsPerVCode[] Events => _werdBepaaldScenario.Events.Union(
    [
        new EventsPerVCode(_werdBepaaldScenario.VerenigingWerdGeregistreerd.VCode, WerkingsgebiedenWerdenNietBepaald),
    ]).ToArray();
}
