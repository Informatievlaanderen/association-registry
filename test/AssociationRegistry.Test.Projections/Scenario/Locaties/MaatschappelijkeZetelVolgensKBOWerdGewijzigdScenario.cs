    namespace AssociationRegistry.Test.Projections.Scenario.Locaties;

using Events;
using AutoFixture;

public class MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario: ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKboFirst { get; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKboSecond { get; }
    public MaatschappelijkeZetelVolgensKBOWerdGewijzigd MaatschappelijkeZetelVolgensKBOWerdGewijzigdFirstLocation { get; }

    public MaatschappelijkeZetelVolgensKBOWerdGewijzigdScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        MaatschappelijkeZetelWerdOvergenomenUitKboFirst = AutoFixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
        MaatschappelijkeZetelWerdOvergenomenUitKboSecond = AutoFixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>()
            with
            {
                Locatie = AutoFixture.Create<Registratiedata.Locatie>() with
                {
                    LocatieId = MaatschappelijkeZetelWerdOvergenomenUitKboFirst.Locatie.LocatieId + 1,
                }
            };
        MaatschappelijkeZetelVolgensKBOWerdGewijzigdFirstLocation = AutoFixture.Create<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>() with
        {
            LocatieId = MaatschappelijkeZetelWerdOvergenomenUitKboFirst.Locatie.LocatieId,
        };
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, MaatschappelijkeZetelWerdOvergenomenUitKboFirst, MaatschappelijkeZetelWerdOvergenomenUitKboSecond, MaatschappelijkeZetelVolgensKBOWerdGewijzigdFirstLocation)
    ];
}
