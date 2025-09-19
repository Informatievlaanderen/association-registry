namespace AssociationRegistry.Test.Projections.Scenario.Registratie;

using Events;
using AutoFixture;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaDubbelsScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdNaDubbelsScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                with
                {
                    DuplicaatDetectieInfo = AutoFixture.Create<Registratiedata.DuplicaatDetectieInfo>()
                };
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd),
    ];
}
