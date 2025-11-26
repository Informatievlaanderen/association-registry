namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using Events;
using AutoFixture;

public class AdresWerdOvergenomenUitAdressenregisterScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresWerdOvergenomenUitAdressenregister AdresWerdOvergenomenUitAdressenregister { get; }

    public AdresWerdOvergenomenUitAdressenregisterScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AdresWerdOvergenomenUitAdressenregister = AutoFixture.Create<AdresWerdOvergenomenUitAdressenregister>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
            LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };
    }

    public override string AggregateId => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingWerdGeregistreerd, AdresWerdOvergenomenUitAdressenregister),
    ];
}
