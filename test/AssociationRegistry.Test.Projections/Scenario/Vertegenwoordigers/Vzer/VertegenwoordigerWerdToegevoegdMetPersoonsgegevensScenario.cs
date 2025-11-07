namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using Events;
using AutoFixture;

public class VertegenwoordigerWerdToegevoegdMetPersoonsgegevensScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegdMetPersoonsgegevens VertegenwoordigerWerdToegevoegdMetPersoonsgegevens { get; }

    public VertegenwoordigerWerdToegevoegdMetPersoonsgegevensScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        VertegenwoordigerWerdToegevoegdMetPersoonsgegevens = AutoFixture.Create<VertegenwoordigerWerdToegevoegdMetPersoonsgegevens>();
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdToegevoegdMetPersoonsgegevens),
    ];
}
