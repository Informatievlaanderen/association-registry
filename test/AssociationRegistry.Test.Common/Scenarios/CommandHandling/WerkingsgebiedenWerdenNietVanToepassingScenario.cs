namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;
using Vereniging;

public class WerkingsgebiedenWerdenNietVanToepassingScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly WerkingsgebiedenWerdenNietVanToepassing WerkingsgebiedenWerdenNietVanToepassing;

    public WerkingsgebiedenWerdenNietVanToepassingScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        WerkingsgebiedenWerdenNietVanToepassing = fixture.Create<WerkingsgebiedenWerdenNietVanToepassing>() with
        {
            VCode = VCode,
        };
    }

    public override IEnumerable<IEvent> Events()
        =>
        [
            FeitelijkeVerenigingWerdGeregistreerd,
            WerkingsgebiedenWerdenNietVanToepassing,
        ];
}
