namespace AssociationRegistry.Test.Projections.Scenario.RechtsvormWerdGewijzigdInKBO;

using AssociationRegistry.Events;
using AutoFixture;

public class RechtsvormWerdGewijzigdInKBOScenario : ScenarioBase
{
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; }
    public RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO { get; }

    public RechtsvormWerdGewijzigdInKBOScenario()
    {
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();

        RechtsvormWerdGewijzigdInKBO = AutoFixture.Create<RechtsvormWerdGewijzigdInKBO>();
    }

    public override string AggregateId => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            RechtsvormWerdGewijzigdInKBO
        ),
    ];
}
