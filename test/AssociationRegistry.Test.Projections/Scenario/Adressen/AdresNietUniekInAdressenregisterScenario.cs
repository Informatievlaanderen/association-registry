namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using Events;
using AutoFixture;
using System.Linq;

public class AdresNietUniekInAdressenregisterScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresNietUniekInAdressenregister AdresNietUniekInAdressenregister { get; }

    public AdresNietUniekInAdressenregisterScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AdresNietUniekInAdressenregister = AutoFixture.Create<AdresNietUniekInAdressenregister>() with
        {
            LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, AdresNietUniekInAdressenregister),
    ];
}
