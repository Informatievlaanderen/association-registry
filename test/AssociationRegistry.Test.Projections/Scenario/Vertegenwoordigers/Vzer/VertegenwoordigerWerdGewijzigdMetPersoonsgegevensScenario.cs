namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using Events;
using AutoFixture;

public class VertegenwoordigerWerdGewijzigdMetPersoonsgegevensScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdGewijzigdMetPersoonsgegevens VertegenwoordigerWerdGewijzigd { get; }

    public VertegenwoordigerWerdGewijzigdMetPersoonsgegevensScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        VertegenwoordigerWerdGewijzigd = AutoFixture.Create<VertegenwoordigerWerdGewijzigdMetPersoonsgegevens>() with
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
