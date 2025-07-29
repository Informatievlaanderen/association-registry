    namespace AssociationRegistry.Test.Projections.Scenario.Locaties;

using Events;
using AutoFixture;

public class MaatschappelijkeZetelWerdVerwijderdUitKboScenario: ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKboFirst { get; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKboSecond { get; }
    public MaatschappelijkeZetelWerdVerwijderdUitKbo MaatschappelijkeZetelWerdVerwijderdUitKboFirstLocation { get; }

    public MaatschappelijkeZetelWerdVerwijderdUitKboScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        MaatschappelijkeZetelWerdOvergenomenUitKboFirst = AutoFixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
        MaatschappelijkeZetelWerdOvergenomenUitKboSecond = AutoFixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
        MaatschappelijkeZetelWerdVerwijderdUitKboFirstLocation = AutoFixture.Create<MaatschappelijkeZetelWerdVerwijderdUitKbo>() with
        {
            Locatie = AutoFixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = MaatschappelijkeZetelWerdOvergenomenUitKboFirst.Locatie.LocatieId,
            }
        };
    }

    public override string VCode => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, MaatschappelijkeZetelWerdOvergenomenUitKboFirst, MaatschappelijkeZetelWerdOvergenomenUitKboSecond, MaatschappelijkeZetelWerdVerwijderdUitKboFirstLocation)
    ];
}
