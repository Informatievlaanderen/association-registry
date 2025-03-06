namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using Events;
using AutoFixture;
using System.Linq;

public class AdresKonNietOvergenomenWordenUitAdressenregisterScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresKonNietOvergenomenWordenUitAdressenregister AdresKonNietOvergenomenWordenUitAdressenregister { get; }

    public AdresKonNietOvergenomenWordenUitAdressenregisterScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AdresKonNietOvergenomenWordenUitAdressenregister = AutoFixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
        {
            LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, AdresKonNietOvergenomenWordenUitAdressenregister),
    ];
}
