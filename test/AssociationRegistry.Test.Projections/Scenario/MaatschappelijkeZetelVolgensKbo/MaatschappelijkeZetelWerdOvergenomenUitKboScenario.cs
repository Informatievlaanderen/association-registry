namespace AssociationRegistry.Test.Projections.Scenario.MaatschappelijkeZetelVolgensKbo;

using AutoFixture;
using Events;

public class MaatschappelijkeZetelWerdOvergenomenUitKboScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo { get; }

    public MaatschappelijkeZetelWerdOvergenomenUitKboScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        MaatschappelijkeZetelWerdOvergenomenUitKbo = AutoFixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, MaatschappelijkeZetelWerdOvergenomenUitKbo)
    ];
}
