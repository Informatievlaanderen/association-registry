namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using Events;
using Framework;

public class WerkingsgebiedenWerdenNietVanToepassingScenario : ScenarioBase
{
    private readonly WerkingsgebiedenWerdenBepaaldScenario _werdBepaaldScenario = new();
    public WerkingsgebiedenWerdenNietVanToepassing WerkingsgebiedenWerdenNietVanToepassing { get; }

    public WerkingsgebiedenWerdenNietVanToepassingScenario()
    {
        WerkingsgebiedenWerdenNietVanToepassing =
            new WerkingsgebiedenWerdenNietVanToepassing(_werdBepaaldScenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override EventsPerVCode[] Events => _werdBepaaldScenario.Events.Union(
    [
        new EventsPerVCode(_werdBepaaldScenario.VerenigingWerdGeregistreerd.VCode, WerkingsgebiedenWerdenNietVanToepassing),
    ]).ToArray();
}
