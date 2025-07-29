namespace AssociationRegistry.Test.Projections.Scenario.Startdatum.Vzer;

using Events;
using AutoFixture;

public class StartdatumWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public StartdatumWerdGewijzigd StartdatumWerdGewijzigd { get; }

    public StartdatumWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        StartdatumWerdGewijzigd = AutoFixture.Create<StartdatumWerdGewijzigd>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            StartdatumWerdGewijzigd
        ),
    ];
}
