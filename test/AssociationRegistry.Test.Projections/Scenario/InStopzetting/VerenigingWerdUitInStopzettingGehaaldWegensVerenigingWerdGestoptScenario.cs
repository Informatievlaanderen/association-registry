namespace AssociationRegistry.Test.Projections.Scenario.InStopzetting;

using AssociationRegistry.Events;
using AutoFixture;

public class VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdInStopzettingGeplaatst VerenigingWerdInStopzettingGeplaatst { get; }
    public VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt { get; }

    public VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestoptScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdInStopzettingGeplaatst = AutoFixture.Create<VerenigingWerdInStopzettingGeplaatst>();
        VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt =
            AutoFixture.Create<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdInStopzettingGeplaatst,
                VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGestopt
            ),
        ];
}
