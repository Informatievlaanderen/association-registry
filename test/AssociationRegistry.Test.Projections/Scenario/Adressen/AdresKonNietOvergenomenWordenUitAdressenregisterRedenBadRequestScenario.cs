namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using AutoFixture;
using Events;
using Integrations.Grar.Clients;

public class AdresKonNietOvergenomenWordenUitAdressenregisterRedenBadRequestScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public LocatieWerdVerwijderd LocatieWerdVerwijderd { get; }
    public AdresKonNietOvergenomenWordenUitAdressenregister AdresKonNietOvergenomenWordenUitAdressenregister { get; }

    public AdresKonNietOvergenomenWordenUitAdressenregisterRedenBadRequestScenario()
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
                Reden = GrarClient.BadRequestSuccessStatusCodeMessage,
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
