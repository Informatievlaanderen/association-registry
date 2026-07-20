namespace AssociationRegistry.Test.Projections.Scenario.InStopzetting;

using AssociationRegistry.Events;
using AutoFixture;

public class VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdInStopzettingGeplaatst VerenigingWerdInStopzettingGeplaatst { get; }
    public VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel { get; }

    public VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbelScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdInStopzettingGeplaatst = AutoFixture.Create<VerenigingWerdInStopzettingGeplaatst>();
        VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel =
            AutoFixture.Create<VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                VerenigingWerdInStopzettingGeplaatst,
                VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel
            ),
        ];
}
