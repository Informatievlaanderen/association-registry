namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario : CommandhandlerScenarioBase
{
    public FeitelijkeVerenigingWerdGeregistreerdWithALocatieScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = fixture.Create<VCode>();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode, Locaties = Array.Empty<Registratiedata.Locatie>(),
        };

        LocatieWerdToegevoegd = fixture.Create<LocatieWerdToegevoegd>();
    }

    public override VCode VCode { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public LocatieWerdToegevoegd LocatieWerdToegevoegd { get; }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdToegevoegd,
        };
    }
}
