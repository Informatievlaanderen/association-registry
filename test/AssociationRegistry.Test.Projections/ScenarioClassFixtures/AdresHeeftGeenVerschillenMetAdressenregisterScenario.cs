namespace AssociationRegistry.Test.Projections.ScenarioClassFixtures;

using AutoFixture;
using Events;
using Framework;

public class AdresHeeftGeenVerschillenMetAdressenregisterScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresHeeftGeenVerschillenMetAdressenregister AdresHeeftGeenVerschillenMetAdressenregister { get; }

    public AdresHeeftGeenVerschillenMetAdressenregisterScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AdresHeeftGeenVerschillenMetAdressenregister = AutoFixture.Create<AdresHeeftGeenVerschillenMetAdressenregister>();
    }

    public override EventsPerVCode[] Events =>
    [
        new(VerenigingWerdGeregistreerd.VCode, VerenigingWerdGeregistreerd, AdresHeeftGeenVerschillenMetAdressenregister),
    ];
}
