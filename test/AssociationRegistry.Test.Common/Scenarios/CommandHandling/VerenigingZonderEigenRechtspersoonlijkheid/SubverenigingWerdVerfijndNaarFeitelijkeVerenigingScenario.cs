namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using global::AutoFixture;

public class SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging;
    public readonly VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging;

    public SubverenigingWerdVerfijndNaarFeitelijkeVerenigingScenario(VCode vCodeAndereVereniging)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        VerenigingssubtypeWerdVerfijndNaarSubvereniging = new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
            VCode,
            new Registratiedata.SubverenigingVan(vCodeAndereVereniging, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>()));

        VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging = new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(VCode);
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingssubtypeWerdVerfijndNaarSubvereniging,
            VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging
        };
}
