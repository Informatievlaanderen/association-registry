namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using AutoFixture;
using Events;

public class AdresHeeftGeenVerschillenMetAdressenregisterScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresHeeftGeenVerschillenMetAdressenregister AdresHeeftGeenVerschillenMetAdressenregister { get; }

    public AdresHeeftGeenVerschillenMetAdressenregisterScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AdresHeeftGeenVerschillenMetAdressenregister =
            AutoFixture.Create<AdresHeeftGeenVerschillenMetAdressenregister>() with
            {
                LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
            };
    }

    public override string AggregateId => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [new(AggregateId, VerenigingWerdGeregistreerd, AdresHeeftGeenVerschillenMetAdressenregister)];
}
