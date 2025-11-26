namespace AssociationRegistry.Test.Projections.Scenario.Doelgroepen;

using AutoFixture;
using Events;

public class DoelgroepWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public DoelgroepWerdGewijzigd DoelgroepWerdGewijzigd { get; }

    public DoelgroepWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        DoelgroepWerdGewijzigd = AutoFixture.Create<DoelgroepWerdGewijzigd>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            DoelgroepWerdGewijzigd
        ),
    ];
}
