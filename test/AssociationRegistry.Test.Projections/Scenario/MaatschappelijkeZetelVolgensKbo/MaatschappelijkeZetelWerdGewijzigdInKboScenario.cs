namespace AssociationRegistry.Test.Projections.Scenario.MaatschappelijkeZetelVolgensKbo;

using AutoFixture;
using Events;

public class MaatschappelijkeZetelWerdGewijzigdInKboScenario : ScenarioBase
{
    public MaatschappelijkeZetelWerdOvergenomenUitKboScenario MaatschappelijkeZetelWerdOvergenomenUitKboScenario = new();
    public MaatschappelijkeZetelWerdGewijzigdInKbo MaatschappelijkeZetelWerdGewijzigdInKbo { get; }

    public MaatschappelijkeZetelWerdGewijzigdInKboScenario()
    {
        MaatschappelijkeZetelWerdGewijzigdInKbo = AutoFixture.Create<MaatschappelijkeZetelWerdGewijzigdInKbo>() with
        {
            Locatie = AutoFixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = MaatschappelijkeZetelWerdOvergenomenUitKboScenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
            }
        };
    }

    public override string VCode => MaatschappelijkeZetelWerdOvergenomenUitKboScenario.VCode;

    public override EventsPerVCode[] Events => MaatschappelijkeZetelWerdOvergenomenUitKboScenario.Events.Union(
    [
        new EventsPerVCode(VCode, MaatschappelijkeZetelWerdGewijzigdInKbo)
    ]).ToArray();
}
