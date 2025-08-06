namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using global::AutoFixture;

public class FeitelijkeVerenigingWerdGeregistreerdWithMultipleLocatiesScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerdWithMultipleLocatiesScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = fixture.Create<VCode>();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode, Locaties = Array.Empty<Registratiedata.Locatie>(),
        };

        LocatieWerdToegevoegd = fixture.Create<LocatieWerdToegevoegd>();
        LocatieWerdToegevoegd2 = fixture.Create<LocatieWerdToegevoegd>();
    }

    public override VCode VCode { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public LocatieWerdToegevoegd LocatieWerdToegevoegd { get; }
    public LocatieWerdToegevoegd LocatieWerdToegevoegd2 { get; }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdToegevoegd,
            LocatieWerdToegevoegd2
        };
    }
}
