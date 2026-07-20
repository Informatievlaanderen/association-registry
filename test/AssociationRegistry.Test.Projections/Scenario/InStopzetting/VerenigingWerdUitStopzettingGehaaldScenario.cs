namespace AssociationRegistry.Test.Projections.Scenario.InStopzetting;

using AutoFixture;
using Events;

public class VerenigingWerdUitStopzettingGehaaldScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdInStopzettingGeplaatst VerenigingWerdInStopzettingGeplaatst { get; }
    public VerenigingWerdUitStopzettingGehaald VerenigingWerdUitStopzettingGehaald { get; }

    public VerenigingWerdUitStopzettingGehaaldScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdInStopzettingGeplaatst = AutoFixture.Create<VerenigingWerdInStopzettingGeplaatst>();
        VerenigingWerdUitStopzettingGehaald = AutoFixture.Create<VerenigingWerdUitStopzettingGehaald>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdInStopzettingGeplaatst,
                VerenigingWerdUitStopzettingGehaald
            ),
        ];
}
