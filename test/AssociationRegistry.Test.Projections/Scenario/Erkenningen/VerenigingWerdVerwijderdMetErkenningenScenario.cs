namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using AutoFixture;
using Events;

public class VerenigingWerdVerwijderdMetErkenningenScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd EersteErkenningWerdGeregistreerd { get; }
    public ErkenningWerdGeregistreerd TweedeErkenningWerdGeregistreerd { get; }
    public VerenigingWerdVerwijderd VerenigingWerdVerwijderd { get; }

    public VerenigingWerdVerwijderdMetErkenningenScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        EersteErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();
        TweedeErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();

        VerenigingWerdVerwijderd = AutoFixture.Create<VerenigingWerdVerwijderd>() with
        {
            VCode = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
                EersteErkenningWerdGeregistreerd,
                TweedeErkenningWerdGeregistreerd,
                VerenigingWerdVerwijderd
            ),
        ];
}
