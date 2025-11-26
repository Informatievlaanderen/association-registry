namespace AssociationRegistry.Test.Projections.Scenario.Werkingsgebieden;

using Events;

public class WerkingsgebiedenWerdenNietVanToepassingScenario : ScenarioBase
{
    private readonly WerkingsgebiedenWerdenBepaaldScenario _werdBepaaldScenario = new();
    public WerkingsgebiedenWerdenNietVanToepassing WerkingsgebiedenWerdenNietVanToepassing { get; }

    public WerkingsgebiedenWerdenNietVanToepassingScenario()
    {
        WerkingsgebiedenWerdenNietVanToepassing =
            new WerkingsgebiedenWerdenNietVanToepassing(_werdBepaaldScenario.VerenigingWerdGeregistreerd.VCode);
    }

    public override string AggregateId => _werdBepaaldScenario.AggregateId;

    public override EventsPerVCode[] Events => _werdBepaaldScenario.Events.Union(
    [
        new EventsPerVCode(AggregateId, WerkingsgebiedenWerdenNietVanToepassing),
    ]).ToArray();
}
