namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using AutoFixture;
using Events;

public class SchorsingVanErkenningWerdOpgehevenScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeschorst ErkenningWerdGeschorst { get; }
    public SchorsingVanErkenningWerdOpgeheven SchorsingVanErkenningWerdOpgeheven { get; }

    public SchorsingVanErkenningWerdOpgehevenScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();
        ErkenningWerdGeschorst = AutoFixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };

        SchorsingVanErkenningWerdOpgeheven = AutoFixture.Create<SchorsingVanErkenningWerdOpgeheven>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                ErkenningWerdGeregistreerd,
                ErkenningWerdGeschorst,
                SchorsingVanErkenningWerdOpgeheven
            ),
        ];
}
