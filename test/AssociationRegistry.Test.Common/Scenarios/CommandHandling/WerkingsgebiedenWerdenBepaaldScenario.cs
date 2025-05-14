namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AutoFixture;
using Events;
using global::AutoFixture;
using Vereniging;

public class WerkingsgebiedenWerdenBepaaldScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald;

    public WerkingsgebiedenWerdenBepaaldScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        WerkingsgebiedenWerdenBepaald = fixture.Create<WerkingsgebiedenWerdenBepaald>() with
        {
            VCode = VCode,
        };
    }

    public override IEnumerable<IEvent> Events()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            WerkingsgebiedenWerdenBepaald,
        ];
}
