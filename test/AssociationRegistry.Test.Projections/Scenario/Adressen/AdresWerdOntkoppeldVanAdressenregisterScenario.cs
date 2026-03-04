namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using AutoFixture;
using Events;

public class AdresWerdOntkoppeldVanAdressenregisterScenario : ScenarioBase
{
    public readonly AdresWerdOvergenomenUitAdressenregisterScenario AdresWerdOvergenomenUitAdressenregisterScenario =
        new();

    public AdresWerdOntkoppeldVanAdressenregister AdresWerdOntkoppeldVanAdressenregister { get; }

    public AdresWerdOntkoppeldVanAdressenregisterScenario()
    {
        AdresWerdOntkoppeldVanAdressenregister = AutoFixture.Create<AdresWerdOntkoppeldVanAdressenregister>() with
        {
            VCode = AdresWerdOvergenomenUitAdressenregisterScenario.AggregateId,
            LocatieId = AdresWerdOvergenomenUitAdressenregisterScenario
                .AdresWerdOvergenomenUitAdressenregister
                .LocatieId,
        };
    }

    public override string AggregateId => AdresWerdOntkoppeldVanAdressenregister.VCode;

    public override EventsPerVCode[] Events =>
        AdresWerdOvergenomenUitAdressenregisterScenario
            .Events.Union([new EventsPerVCode(AggregateId, AdresWerdOntkoppeldVanAdressenregister)])
            .ToArray();
}
