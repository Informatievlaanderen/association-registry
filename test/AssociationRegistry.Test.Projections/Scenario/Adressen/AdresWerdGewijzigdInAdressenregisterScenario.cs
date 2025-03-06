namespace AssociationRegistry.Test.Projections.Scenario.Adressen;

using Events;
using AutoFixture;
using System.Linq;

public class AdresWerdGewijzigdInAdressenregisterScenario : ScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresWerdGewijzigdInAdressenregister AdresWerdGewijzigdInAdressenregister { get; }

    public AdresWerdGewijzigdInAdressenregisterScenario()
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AdresWerdGewijzigdInAdressenregister = AutoFixture.Create<AdresWerdGewijzigdInAdressenregister>() with
        {
            VCode = VerenigingWerdGeregistreerd.VCode,
            LocatieId = VerenigingWerdGeregistreerd.Locaties.First().LocatieId,
        };
    }

    public override string VCode => VerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(VCode, VerenigingWerdGeregistreerd, AdresWerdGewijzigdInAdressenregister),
    ];
}
