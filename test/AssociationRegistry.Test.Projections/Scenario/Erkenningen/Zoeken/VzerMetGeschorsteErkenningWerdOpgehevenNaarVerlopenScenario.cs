namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen.Zoeken;

using AutoFixture;
using Events;

public class VzerMetGeschorsteErkenningWerdOpgehevenNaarVerlopenScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeschorst ErkenningWerdGeschorst { get; }
    public ErkenningWerdVerlopen ErkenningWerdVerlopen { get; }
    public SchorsingVanErkenningWerdOpgeheven SchorsingVanErkenningWerdOpgeheven { get; }

    public VzerMetGeschorsteErkenningWerdOpgehevenNaarVerlopenScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        ErkenningWerdVerlopen = AutoFixture.Create<ErkenningWerdVerlopen>();

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
                ErkenningWerdVerlopen,
                ErkenningWerdGeschorst,
                SchorsingVanErkenningWerdOpgeheven
            ),
        ];
}
