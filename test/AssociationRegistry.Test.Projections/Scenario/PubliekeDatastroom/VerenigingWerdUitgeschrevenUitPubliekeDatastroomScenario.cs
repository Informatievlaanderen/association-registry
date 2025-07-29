namespace AssociationRegistry.Test.Projections.Scenario.PubliekeDatastroom;

using Events;
using AutoFixture;

public class VerenigingWerdUitgeschrevenUitPubliekeDatastroomScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdUitgeschrevenUitPubliekeDatastroom VerenigingWerdUitgeschrevenUitPubliekeDatastroom { get; }
    public VerenigingWerdIngeschrevenInPubliekeDatastroom VerenigingWerdIngeschrevenInPubliekeDatastroom { get; }

    public VerenigingWerdUitgeschrevenUitPubliekeDatastroomScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdIngeschrevenInPubliekeDatastroom = AutoFixture.Create<VerenigingWerdIngeschrevenInPubliekeDatastroom>();
        VerenigingWerdUitgeschrevenUitPubliekeDatastroom = AutoFixture.Create<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingWerdIngeschrevenInPubliekeDatastroom,
            VerenigingWerdUitgeschrevenUitPubliekeDatastroom
        ),
    ];
}
