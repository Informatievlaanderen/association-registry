namespace AssociationRegistry.Test.Projections.Scenario.NaamWerdGewijzigd.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class NaamWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public NaamWerdGewijzigd NaamWerdGewijzigd { get; }

    public NaamWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        NaamWerdGewijzigd = AutoFixture.Create<NaamWerdGewijzigd>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            NaamWerdGewijzigd
        ),
    ];
}
