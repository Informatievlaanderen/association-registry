namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class VertegenwoordigerWerdGewijzigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdGewijzigd VertegenwoordigerWerdGewijzigd { get; }

    public VertegenwoordigerWerdGewijzigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        VertegenwoordigerWerdGewijzigd = AutoFixture.Create<VertegenwoordigerWerdGewijzigd>() with
        {
            VertegenwoordigerId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.First().VertegenwoordigerId,
        };
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdGewijzigd),
    ];
}
