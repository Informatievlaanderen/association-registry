namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using AutoFixture;
using Events;

public class ErkenningWerdVerwijderdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerdToBeRemoved { get; }
    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdVerwijderd ErkenningWerdVerwijderd { get; }

    public ErkenningWerdVerwijderdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerdToBeRemoved = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        ErkenningWerdVerwijderd = AutoFixture.Create<ErkenningWerdVerwijderd>() with
        {
            ErkenningId = ErkenningWerdGeregistreerdToBeRemoved.ErkenningId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                ErkenningWerdGeregistreerdToBeRemoved,
                ErkenningWerdGeregistreerd,
                ErkenningWerdVerwijderd
            ),
        ];
}
