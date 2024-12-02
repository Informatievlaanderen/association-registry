namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class MultipleAdresWerdOvergenomenUitAdressenregisterScenario : CommandhandlerScenarioBase
{
    public MultipleAdresWerdOvergenomenUitAdressenregisterScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = fixture.Create<VCode>();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode, Locaties = Array.Empty<Registratiedata.Locatie>(),
        };

        LocatieWerdToegevoegd = fixture.Create<LocatieWerdToegevoegd>();
        AdresWerdOvergenomenUitAdressenregister = fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
            with
            {
                LocatieId = LocatieWerdToegevoegd.Locatie.LocatieId,
            };

        LocatieWerdToegevoegd2 = fixture.Create<LocatieWerdToegevoegd>();
        AdresWerdOvergenomenUitAdressenregister2 = fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
            with
            {
                LocatieId = LocatieWerdToegevoegd2.Locatie.LocatieId,
            };
    }

    public override VCode VCode { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public LocatieWerdToegevoegd LocatieWerdToegevoegd { get; }
    public AdresWerdOvergenomenUitAdressenregister AdresWerdOvergenomenUitAdressenregister { get; }
    public LocatieWerdToegevoegd LocatieWerdToegevoegd2 { get; }
    public AdresWerdOvergenomenUitAdressenregister AdresWerdOvergenomenUitAdressenregister2 { get; }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdToegevoegd,
            AdresWerdOvergenomenUitAdressenregister,
            LocatieWerdToegevoegd2,
            AdresWerdOvergenomenUitAdressenregister2,
        };
    }
}
