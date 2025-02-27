namespace AssociationRegistry.Test.Projections.Scenario.KorteBeschijving;

using AssociationRegistry.Events;
using AutoFixture;

public class KorteBeschrijvingWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public KorteBeschrijvingWerdGewijzigd KorteBeschrijvingWerdGewijzigd { get; }

    public KorteBeschrijvingWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        KorteBeschrijvingWerdGewijzigd = AutoFixture.Create<KorteBeschrijvingWerdGewijzigd>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            KorteBeschrijvingWerdGewijzigd
        ),
    ];
}
