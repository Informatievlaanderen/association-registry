namespace AssociationRegistry.Test.Projections.Scenario.NaamWerdGewijzigd.Kbo;

using Events;
using AutoFixture;

public class NaamWerdGewijzigdInKboScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public NaamWerdGewijzigdInKbo NaamWerdGewijzigdInKbo { get; }

    public NaamWerdGewijzigdInKboScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        NaamWerdGewijzigdInKbo = AutoFixture.Create<NaamWerdGewijzigdInKbo>();
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            NaamWerdGewijzigdInKbo
        ),
    ];
}
