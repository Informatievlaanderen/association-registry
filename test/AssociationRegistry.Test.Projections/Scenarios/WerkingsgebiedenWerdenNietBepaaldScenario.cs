namespace AssociationRegistry.Test.Projections.Scenarios;

using Events;

public class WerkingsgebiedenWerdenNietBepaaldScenario : IScenario
{
    private readonly WerkingsgebiedenWerdenBepaaldScenario _scenario;
    public WerkingsgebiedenWerdenNietBepaald WerkingsgebiedenWerdenNietBepaald { get; }

    public WerkingsgebiedenWerdenNietBepaaldScenario()
    {
        _scenario = new WerkingsgebiedenWerdenBepaaldScenario();

        WerkingsgebiedenWerdenNietBepaald = new WerkingsgebiedenWerdenNietBepaald(_scenario.WerkingsgebiedenWerdenBepaald.VCode);
    }

    public string VCode => WerkingsgebiedenWerdenNietBepaald.VCode;

    public IEnumerable<EventsPerVCode> GivenEvents =>
    _scenario.GivenEvents.Append(
        new EventsPerVCode(WerkingsgebiedenWerdenNietBepaald.VCode,
                           WerkingsgebiedenWerdenNietBepaald));
}
