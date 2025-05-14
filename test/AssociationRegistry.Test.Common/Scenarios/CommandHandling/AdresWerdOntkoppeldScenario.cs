namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class AdresWerdOntkoppeldScenario : CommandhandlerScenarioBase
{
    public AdresWerdOntkoppeldScenario()
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

        AdresWerdOntkoppeldVanAdressenregister = fixture.Create<AdresWerdOntkoppeldVanAdressenregister>()
            with
            {
                LocatieId = LocatieWerdToegevoegd.Locatie.LocatieId,
            };
    }

    public override VCode VCode { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public LocatieWerdToegevoegd LocatieWerdToegevoegd { get; }
    public AdresWerdOvergenomenUitAdressenregister AdresWerdOvergenomenUitAdressenregister { get; }
    public AdresWerdOntkoppeldVanAdressenregister AdresWerdOntkoppeldVanAdressenregister { get; }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdToegevoegd,
            AdresWerdOvergenomenUitAdressenregister,
            AdresWerdOntkoppeldVanAdressenregister
        };
    }
}
