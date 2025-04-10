namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class VertegenwoordigerWerdToegevoegdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; }

    public VertegenwoordigerWerdToegevoegdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        VertegenwoordigerWerdToegevoegd = AutoFixture.Create<VertegenwoordigerWerdToegevoegd>() with
        {
            VertegenwoordigerId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.First().VertegenwoordigerId,
        };
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdToegevoegd),
    ];
}
