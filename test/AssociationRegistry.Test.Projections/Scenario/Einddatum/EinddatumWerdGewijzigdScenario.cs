namespace AssociationRegistry.Test.Projections.Scenario.Einddatum;

using Events;
using AutoFixture;

public class EinddatumWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public EinddatumWerdGewijzigd EinddatumWerdGewijzigd { get; }

    public EinddatumWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        EinddatumWerdGewijzigd = AutoFixture.Create<EinddatumWerdGewijzigd>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            EinddatumWerdGewijzigd
        ),
    ];
}
