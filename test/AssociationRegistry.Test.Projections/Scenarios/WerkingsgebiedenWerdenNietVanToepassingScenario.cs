namespace AssociationRegistry.Test.Projections.Scenarios;

using Events;

public class WerkingsgebiedenWerdenNietVanToepassingScenario : IScenario
{
    private readonly WerkingsgebiedenWerdenBepaaldScenario _baseScenario;
    public WerkingsgebiedenWerdenNietVanToepassing WerkingsgebiedenWerdenNietVanToepassing { get; }

    public WerkingsgebiedenWerdenNietVanToepassingScenario()
    {
        _baseScenario = new WerkingsgebiedenWerdenBepaaldScenario();

        WerkingsgebiedenWerdenNietVanToepassing = new WerkingsgebiedenWerdenNietVanToepassing(_baseScenario.WerkingsgebiedenWerdenBepaald.VCode);
    }

    public string VCode => WerkingsgebiedenWerdenNietVanToepassing.VCode;

    public IEnumerable<EventsPerVCode> GivenEvents =>
        _baseScenario.GivenEvents
                     .Append(new EventsPerVCode(WerkingsgebiedenWerdenNietVanToepassing.VCode, WerkingsgebiedenWerdenNietVanToepassing));
}


