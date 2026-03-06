namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using AutoFixture;
using Events;

public class AdresKonNietOvergenomenWordenUitAdressenregisterRedenLocatieVerwijderdScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public LocatieWerdVerwijderd LocatieWerdVerwijderd { get; }
    public AdresKonNietOvergenomenWordenUitAdressenregister AdresKonNietOvergenomenWordenUitAdressenregister { get; }

    public AdresKonNietOvergenomenWordenUitAdressenregisterRedenLocatieVerwijderdScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        LocatieWerdVerwijderd = AutoFixture.Create<LocatieWerdVerwijderd>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
            Locatie = VerenigingWerdGeregistreerd.Locaties.First(),
        };

        AdresKonNietOvergenomenWordenUitAdressenregister =
            AutoFixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
            {
                LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
                Reden = AdresKonNietOvergenomenWordenUitAdressenregister.RedenLocatieWerdVerwijderd,
            };
    }

    public override string AggregateId => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                VerenigingWerdGeregistreerd,
                LocatieWerdVerwijderd,
                AdresKonNietOvergenomenWordenUitAdressenregister
            ),
        ];
}
