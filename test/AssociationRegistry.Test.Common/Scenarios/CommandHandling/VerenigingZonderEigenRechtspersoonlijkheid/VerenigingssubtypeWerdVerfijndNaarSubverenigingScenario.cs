namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using global::AutoFixture;

public class VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging;

    public VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        VerenigingssubtypeWerdVerfijndNaarSubvereniging = new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
            VCode,
            new Registratiedata.SubverenigingVan(VCode.Create("V0009003").Value, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>()));
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingssubtypeWerdVerfijndNaarSubvereniging
        };
}
