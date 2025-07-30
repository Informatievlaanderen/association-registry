namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using Events;
using AutoFixture;

public class AdresWerdOntkoppeldVanAdressenregisterScenario : ScenarioBase
{
    private readonly AdresWerdOvergenomenUitAdressenregisterScenario _adresWerdOvergenomenUitAdressenregisterScenario = new();

    public AdresWerdOntkoppeldVanAdressenregister AdresWerdOntkoppeldVanAdressenregister { get; }

    public AdresWerdOntkoppeldVanAdressenregisterScenario()
    {
        AdresWerdOntkoppeldVanAdressenregister = AutoFixture.Create<AdresWerdOntkoppeldVanAdressenregister>() with
        {
            VCode = _adresWerdOvergenomenUitAdressenregisterScenario.VCode,
            LocatieId = _adresWerdOvergenomenUitAdressenregisterScenario.AdresWerdOvergenomenUitAdressenregister.LocatieId,
        };
    }

    public override string VCode => AdresWerdOntkoppeldVanAdressenregister.VCode;

    public override EventsPerVCode[] Events => _adresWerdOvergenomenUitAdressenregisterScenario.Events///.Union(
    // [
    //     new EventsPerVCode(VCode, AdresWerdOntkoppeldVanAdressenregister),
    // ])
    .ToArray();
}
