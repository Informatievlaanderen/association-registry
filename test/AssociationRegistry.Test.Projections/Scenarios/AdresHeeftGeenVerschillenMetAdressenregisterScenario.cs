namespace AssociationRegistry.Test.Projections.Scenarios;

using AutoFixture;
using Common.AutoFixture;
using Events;

public class AdresHeeftGeenVerschillenMetAdressenregisterScenario : IScenario
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresHeeftGeenVerschillenMetAdressenregister AdresHeeftGeenVerschillenMetAdressenregister { get; set; }

    public AdresHeeftGeenVerschillenMetAdressenregisterScenario()
    {
        var fixture = new Fixture().CustomizeDomain();
        VerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        AdresHeeftGeenVerschillenMetAdressenregister = fixture.Create<AdresHeeftGeenVerschillenMetAdressenregister>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
        };
    }

    public string VCode => VerenigingWerdGeregistreerd.VCode;

    public IEnumerable<EventsPerVCode> GivenEvents =>
    [
        new EventsPerVCode(VerenigingWerdGeregistreerd.VCode,
                           VerenigingWerdGeregistreerd, AdresHeeftGeenVerschillenMetAdressenregister),
    ];
}
