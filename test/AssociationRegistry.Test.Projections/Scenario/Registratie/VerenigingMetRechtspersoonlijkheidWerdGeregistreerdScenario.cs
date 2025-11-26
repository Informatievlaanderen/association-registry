namespace AssociationRegistry.Test.Projections.Scenario.Registratie;

using Events;
using AutoFixture;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd),
    ];
}
