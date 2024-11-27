namespace AssociationRegistry.Test.Projections.Scenario;

using Events;

public class WerkingsgebiedenWerdenGewijzigdScenario : ScenarioBase
{
    private readonly WerkingsgebiedenWerdenBepaaldScenario _werdBepaaldScenario = new();
    public WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd { get; }

    public WerkingsgebiedenWerdenGewijzigdScenario()
    {
        WerkingsgebiedenWerdenGewijzigd = new WerkingsgebiedenWerdenGewijzigd(_werdBepaaldScenario.VerenigingWerdGeregistreerd.VCode, [
            new Registratiedata.Werkingsgebied(Code: "BE25535011", Naam: "Middelkerke"),
            new Registratiedata.Werkingsgebied(Code: "BE25535013", Naam: "Oostende"),
            new Registratiedata.Werkingsgebied(Code: "BE25535014", Naam: "Oudenburg"),
        ]);
    }

    public override string VCode => _werdBepaaldScenario.VCode;

    public override EventsPerVCode[] Events => _werdBepaaldScenario.Events.Union(
    [
        new (VCode, WerkingsgebiedenWerdenGewijzigd),
    ]).ToArray();
}
