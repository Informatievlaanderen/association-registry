namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using global::AutoFixture;

public class SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009008");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging;
    public SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging = fixture.Create<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging>() with
        {
            VCode = VCode
        };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging
        };
}
