namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using Events;
using AutoFixture;

public class AdresWerdNietGevondenInAdressenregisterScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresWerdNietGevondenInAdressenregister AdresWerdNietGevondenInAdressenregister { get; }

    public AdresWerdNietGevondenInAdressenregisterScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AdresWerdNietGevondenInAdressenregister = AutoFixture.Create<AdresWerdNietGevondenInAdressenregister>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
            LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, AdresWerdNietGevondenInAdressenregister),
    ];
}
