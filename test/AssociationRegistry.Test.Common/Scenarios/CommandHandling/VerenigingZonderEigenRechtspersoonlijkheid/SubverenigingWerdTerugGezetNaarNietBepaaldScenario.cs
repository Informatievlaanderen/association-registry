namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;
using Vereniging;

public class SubverenigingWerdTerugGezetNaarNietBepaaldScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VerenigingssubtypeWerdVerfijndNaarSubvereniging VerenigingssubtypeWerdVerfijndNaarSubvereniging;
    public readonly VerenigingssubtypeWerdTerugGezetNaarNietBepaald VerenigingssubtypeWerdTerugGezetNaarNietBepaald;

    public SubverenigingWerdTerugGezetNaarNietBepaaldScenario(VCode vCodeAndereVereniging)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = VCode };

        VerenigingssubtypeWerdVerfijndNaarSubvereniging = new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
            VCode,
            new Registratiedata.SubverenigingVan(vCodeAndereVereniging, fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>()));

        VerenigingssubtypeWerdTerugGezetNaarNietBepaald = new VerenigingssubtypeWerdTerugGezetNaarNietBepaald(VCode);
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingssubtypeWerdVerfijndNaarSubvereniging,
            VerenigingssubtypeWerdTerugGezetNaarNietBepaald
        };
}
