namespace AssociationRegistry.Test.Projections.Scenario.Locaties;

using Events;
using AutoFixture;

public class LocatieWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public LocatieWerdGewijzigd LocatieWerdGewijzigd { get; }

    public LocatieWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        LocatieWerdGewijzigd = AutoFixture.Create<LocatieWerdGewijzigd>() with
        {
            Locatie = AutoFixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First().LocatieId,
            },
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, LocatieWerdGewijzigd)
    ];
}
