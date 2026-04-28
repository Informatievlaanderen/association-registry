namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using Events;
using AutoFixture;

public class ErkenningWerdGeschorstScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeschorst ErkenningWerdGeschorst { get; }

    public ErkenningWerdGeschorstScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();
        ErkenningWerdGeschorst = AutoFixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd, ErkenningWerdGeschorst),
    ];
}
