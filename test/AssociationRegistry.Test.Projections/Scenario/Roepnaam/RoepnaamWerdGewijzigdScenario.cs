namespace AssociationRegistry.Test.Projections.Scenario.Roepnaam;

using AssociationRegistry.Events;
using AutoFixture;

public class RoepnaamWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public RoepnaamWerdGewijzigd RoepnaamWerdGewijzigd { get; }

    public RoepnaamWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        RoepnaamWerdGewijzigd = AutoFixture.Create<RoepnaamWerdGewijzigd>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            RoepnaamWerdGewijzigd
        ),
    ];
}
