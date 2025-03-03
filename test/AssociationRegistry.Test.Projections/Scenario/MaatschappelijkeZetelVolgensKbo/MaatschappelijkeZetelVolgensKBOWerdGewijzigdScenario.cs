namespace AssociationRegistry.Test.Projections.Scenario.MaatschappelijkeZetelVolgensKbo;

using AutoFixture;
using Events;

public class MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario : ScenarioBase
{
    public MaatschappelijkeZetelWerdOvergenomenUitKboScenario MaatschappelijkeZetelWerdOvergenomenUitKboScenario = new();
    public MaatschappelijkeZetelVolgensKBOWerdGewijzigd MaatschappelijkeZetelVolgensKBOWerdGewijzigd { get; }

    public MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario()
    {
        MaatschappelijkeZetelVolgensKBOWerdGewijzigd = AutoFixture.Create<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>() with
        {

            LocatieId = MaatschappelijkeZetelWerdOvergenomenUitKboScenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
        };
    }

    public override string VCode => MaatschappelijkeZetelWerdOvergenomenUitKboScenario.VCode;

    public override EventsPerVCode[] Events => MaatschappelijkeZetelWerdOvergenomenUitKboScenario.Events.Union(
    [
        new EventsPerVCode(VCode, MaatschappelijkeZetelVolgensKBOWerdGewijzigd)
    ]).ToArray();
}
