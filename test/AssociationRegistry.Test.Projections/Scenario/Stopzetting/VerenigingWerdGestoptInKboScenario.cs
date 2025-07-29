namespace AssociationRegistry.Test.Projections.Scenario.Stopzetting;

using Events;
using AutoFixture;

public class VerenigingWerdGestoptInKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdGestoptInKBO VerenigingWerdGestoptInKBO { get; }

    public VerenigingWerdGestoptInKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdGestoptInKBO = AutoFixture.Create<VerenigingWerdGestoptInKBO>();
    }

    public override string VCode => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingWerdGestoptInKBO
        ),
    ];
}
