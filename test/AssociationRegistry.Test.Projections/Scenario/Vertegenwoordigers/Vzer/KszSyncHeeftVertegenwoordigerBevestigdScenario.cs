namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using Events;
using AutoFixture;

public class KszSyncHeeftVertegenwoordigerBevestigdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public KszSyncHeeftVertegenwoordigerBevestigd KszSyncHeeftVertegenwoordigerBevestigd { get; }

    public KszSyncHeeftVertegenwoordigerBevestigdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        KszSyncHeeftVertegenwoordigerBevestigd = AutoFixture.Create<KszSyncHeeftVertegenwoordigerBevestigd>()
            with
            {
                VertegenwoordigerId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].VertegenwoordigerId,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, KszSyncHeeftVertegenwoordigerBevestigd),
    ];
}
