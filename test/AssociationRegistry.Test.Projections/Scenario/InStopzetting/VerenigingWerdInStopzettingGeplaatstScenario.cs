namespace AssociationRegistry.Test.Projections.Scenario.InStopzetting;

using AssociationRegistry.Events;
using AutoFixture;

public class VerenigingWerdInStopzettingGeplaatstScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdInStopzettingGeplaatst VerenigingWerdInStopzettingGeplaatst { get; }

    public VerenigingWerdInStopzettingGeplaatstScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdInStopzettingGeplaatst = AutoFixture.Create<VerenigingWerdInStopzettingGeplaatst>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdInStopzettingGeplaatst
            ),
        ];
}
