    namespace AssociationRegistry.Test.Projections.Scenario.Locaties;

using Events;
using AutoFixture;

public class MaatschappelijkeZetelWerdGewijzigdInKboScenario: ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKboFirst { get; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKboSecond { get; }
    public MaatschappelijkeZetelWerdGewijzigdInKbo MaatschappelijkeZetelWerdGewijzigdInKboFromFirstLocation { get; }

    public MaatschappelijkeZetelWerdGewijzigdInKboScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        MaatschappelijkeZetelWerdOvergenomenUitKboFirst = AutoFixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
        MaatschappelijkeZetelWerdOvergenomenUitKboSecond = AutoFixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
        MaatschappelijkeZetelWerdGewijzigdInKboFromFirstLocation = AutoFixture.Create<MaatschappelijkeZetelWerdGewijzigdInKbo>() with
        {
            Locatie = AutoFixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = MaatschappelijkeZetelWerdOvergenomenUitKboFirst.Locatie.LocatieId,
            }
        };
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, MaatschappelijkeZetelWerdOvergenomenUitKboFirst, MaatschappelijkeZetelWerdOvergenomenUitKboSecond, MaatschappelijkeZetelWerdGewijzigdInKboFromFirstLocation)
    ];
}
