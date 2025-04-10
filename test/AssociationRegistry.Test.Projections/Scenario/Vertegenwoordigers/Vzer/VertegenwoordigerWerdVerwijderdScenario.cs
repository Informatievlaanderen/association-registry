namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using AssociationRegistry.Events;
using AutoFixture;

public class VertegenwoordigerWerdVerwijderdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public VertegenwoordigerWerdVerwijderd VertegenwoordigerWerdVerwijderd { get; }

    public VertegenwoordigerWerdVerwijderdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        VertegenwoordigerWerdVerwijderd = AutoFixture.Create<VertegenwoordigerWerdVerwijderd>() with
        {
            VertegenwoordigerId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.First().VertegenwoordigerId
        };
    }

    public override string VCode => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, VertegenwoordigerWerdVerwijderd),
    ];
}
