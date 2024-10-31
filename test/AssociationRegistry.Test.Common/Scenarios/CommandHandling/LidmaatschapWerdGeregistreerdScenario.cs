namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class LidmaatschapWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; }

    public LidmaatschapWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        LidmaatschapWerdToegevoegd =
            new LidmaatschapWerdToegevoegd(FeitelijkeVerenigingWerdGeregistreerd.VCode, fixture.Create<Registratiedata.Lidmaatschap>());
    }

    public override IEnumerable<IEvent> Events()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            LidmaatschapWerdToegevoegd,
        ];
}
