namespace AssociationRegistry.Test.Projections.Scenario.Verwijdering;

using AssociationRegistry.Events;
using AutoFixture;

public class VerenigingWerdVerwijderdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdVerwijderd VerenigingWerdVerwijderd { get; }

    public VerenigingWerdVerwijderdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdVerwijderd = AutoFixture.Create<VerenigingWerdVerwijderd>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingWerdVerwijderd
        ),
    ];
}
