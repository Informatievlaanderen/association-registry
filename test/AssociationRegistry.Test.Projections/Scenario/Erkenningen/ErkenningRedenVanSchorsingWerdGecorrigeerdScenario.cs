namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using AutoFixture;
using Events;

public class ErkenningRedenVanSchorsingWerdGecorrigeerdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeschorst ErkenningWerdGeschorst { get; }
    public ErkenningRedenVanSchorsingWerdGecorrigeerd ErkenningRedenVanSchorsingWerdGecorrigeerd { get; }

    public ErkenningRedenVanSchorsingWerdGecorrigeerdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();
        ErkenningWerdGeschorst = AutoFixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };
        ErkenningRedenVanSchorsingWerdGecorrigeerd =
            AutoFixture.Create<ErkenningRedenVanSchorsingWerdGecorrigeerd>() with
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
                ErkenningRedenVanSchorsingWerdGecorrigeerd
            ),
        ];
}
