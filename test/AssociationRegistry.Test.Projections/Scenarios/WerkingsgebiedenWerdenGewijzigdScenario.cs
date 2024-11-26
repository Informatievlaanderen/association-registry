namespace AssociationRegistry.Test.Projections.Scenarios;

using Events;

public class WerkingsgebiedenWerdenGewijzigdScenario : IScenario
{
    private readonly WerkingsgebiedenWerdenBepaaldScenario _scenario;
    public WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd { get; }

    public WerkingsgebiedenWerdenGewijzigdScenario()
    {
        _scenario = new WerkingsgebiedenWerdenBepaaldScenario();

        WerkingsgebiedenWerdenGewijzigd = new WerkingsgebiedenWerdenGewijzigd(_scenario.WerkingsgebiedenWerdenBepaald.VCode, new[]
        {
            new Registratiedata.Werkingsgebied(Code: "BE25535011", Naam: "Middelkerke"),
            new Registratiedata.Werkingsgebied(Code: "BE25535013", Naam: "Oostende"),
            new Registratiedata.Werkingsgebied(Code: "BE25535014", Naam: "Oudenburg"),
        });
    }

    public string VCode => WerkingsgebiedenWerdenGewijzigd.VCode;

    public IEnumerable<EventsPerVCode> GivenEvents =>
    _scenario.GivenEvents.Append(
        new EventsPerVCode(WerkingsgebiedenWerdenGewijzigd.VCode,
                           WerkingsgebiedenWerdenGewijzigd));
}
