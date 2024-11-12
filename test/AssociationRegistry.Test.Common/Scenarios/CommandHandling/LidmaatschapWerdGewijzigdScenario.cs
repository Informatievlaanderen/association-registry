namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class LidmaatschapWerdGewijzigdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; }
    public LidmaatschapWerdGewijzigd LidmaatschapWerdGewijzigd { get; set; }

    public LidmaatschapWerdGewijzigdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = VCode };

        var lidmaatschap = fixture.Create<Registratiedata.Lidmaatschap>()
            with
            {
                LidmaatschapId = new LidmaatschapId(1),
            };

        LidmaatschapWerdToegevoegd =
            new LidmaatschapWerdToegevoegd(FeitelijkeVerenigingWerdGeregistreerd.VCode, lidmaatschap);

        LidmaatschapWerdGewijzigd =
            new LidmaatschapWerdGewijzigd(FeitelijkeVerenigingWerdGeregistreerd.VCode, lidmaatschap with
            {
                Identificatie = fixture.Create<LidmaatschapIdentificatie>(),
            });
    }

    public override IEnumerable<IEvent> Events()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            LidmaatschapWerdToegevoegd,
            LidmaatschapWerdGewijzigd,
        ];
}
