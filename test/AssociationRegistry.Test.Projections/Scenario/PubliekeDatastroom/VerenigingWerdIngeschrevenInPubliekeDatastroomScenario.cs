namespace AssociationRegistry.Test.Projections.Scenario.PubliekeDatastroom;

using AssociationRegistry.Events;
using AutoFixture;

public class VerenigingWerdIngeschrevenInPubliekeDatastroomScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public VerenigingWerdUitgeschrevenUitPubliekeDatastroom VerenigingWerdUitgeschrevenUitPubliekeDatastroom { get; }
    public VerenigingWerdIngeschrevenInPubliekeDatastroom VerenigingWerdIngeschrevenInPubliekeDatastroom { get; }

    public VerenigingWerdIngeschrevenInPubliekeDatastroomScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VerenigingWerdUitgeschrevenUitPubliekeDatastroom = AutoFixture.Create<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>();
        VerenigingWerdIngeschrevenInPubliekeDatastroom = AutoFixture.Create<VerenigingWerdIngeschrevenInPubliekeDatastroom>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingWerdUitgeschrevenUitPubliekeDatastroom,
            VerenigingWerdIngeschrevenInPubliekeDatastroom
        ),
    ];
}
