namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using AutoFixture;
using Events;

public class VerenigingWerdNietLangerErkendScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public VerenigingWerdErkend VerenigingWerdErkend { get; }
    public VerenigingWerdNietLangerErkend VerenigingWerdNietLangerErkend { get; }

    public VerenigingWerdNietLangerErkendScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();
        VerenigingWerdErkend = AutoFixture.Create<VerenigingWerdErkend>() with { };

        VerenigingWerdNietLangerErkend = AutoFixture.Create<VerenigingWerdNietLangerErkend>();
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                ErkenningWerdGeregistreerd,
                VerenigingWerdErkend,
                VerenigingWerdNietLangerErkend
            ),
        ];
}
