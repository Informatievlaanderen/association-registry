namespace AssociationRegistry.Test.Projections.Scenario.Locaties;

using Events;
using AutoFixture;

public class MaatschappelijkeZetelWerdOvergenomenUitKboScenario: ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo { get; }

    public MaatschappelijkeZetelWerdOvergenomenUitKboScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        MaatschappelijkeZetelWerdOvergenomenUitKbo = AutoFixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
    }

    public override string VCode => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, MaatschappelijkeZetelWerdOvergenomenUitKbo)
    ];
}
