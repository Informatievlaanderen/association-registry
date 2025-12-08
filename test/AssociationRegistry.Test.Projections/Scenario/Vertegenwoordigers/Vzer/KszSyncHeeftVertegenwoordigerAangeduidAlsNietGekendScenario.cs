namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using Events;
using AutoFixture;

public class KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario : InszScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend { get; }

    public KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekendScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend = AutoFixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend>()
            with
            {
                VertegenwoordigerId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].VertegenwoordigerId,
            };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;
    public override string Insz => KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend.Insz;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend),
    ];
}
