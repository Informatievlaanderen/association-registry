namespace AssociationRegistry.Test.Projections.Scenario.KorteNaamWerdGewijzigd.Kbo;

using AutoFixture;
using Events;

public class KorteNaamWerdGewijzigdInKboScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public KorteNaamWerdGewijzigdInKbo KorteNaamWerdGewijzigdInKbo { get; }

    public KorteNaamWerdGewijzigdInKboScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        KorteNaamWerdGewijzigdInKbo = AutoFixture.Create<KorteNaamWerdGewijzigdInKbo>();
    }

    public override string VCode => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            KorteNaamWerdGewijzigdInKbo
        ),
    ];
}
