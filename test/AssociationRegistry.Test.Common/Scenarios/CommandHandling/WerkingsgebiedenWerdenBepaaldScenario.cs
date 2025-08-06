namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;
using Vereniging;

public class WerkingsgebiedenWerdenBepaaldScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald;
    public readonly GeotagsWerdenBepaald GeotagsWerdenBepaald;

    public WerkingsgebiedenWerdenBepaaldScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        WerkingsgebiedenWerdenBepaald = fixture.Create<WerkingsgebiedenWerdenBepaald>() with
        {
            VCode = VCode,
        };

        GeotagsWerdenBepaald = fixture.Create<GeotagsWerdenBepaald>() with
        {
            VCode = VCode,
        };
    }

    public override IEnumerable<IEvent> Events()
        =>
        [
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            WerkingsgebiedenWerdenBepaald,
            GeotagsWerdenBepaald
        ];
}
