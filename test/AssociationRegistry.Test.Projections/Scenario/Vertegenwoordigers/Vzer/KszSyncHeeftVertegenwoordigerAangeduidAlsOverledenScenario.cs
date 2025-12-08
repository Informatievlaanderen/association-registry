namespace AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;

using Events;
using AutoFixture;

public class KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario : InszScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }
    public KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden { get; }

    public KszSyncHeeftVertegenwoordigerAangeduidAlsOverledenScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden = AutoFixture.Create<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden>() with
        {
            VertegenwoordigerId = VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0].VertegenwoordigerId,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;
    public override string Insz => KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden.Insz;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden),
    ];
}
