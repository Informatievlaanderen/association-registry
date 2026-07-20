namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VzerWerdInStopzettingGeplaatstScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    public readonly VerenigingWerdInStopzettingGeplaatst VerenigingWerdInStopzettingGeplaatst;

    public VzerWerdInStopzettingGeplaatstScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        VerenigingWerdInStopzettingGeplaatst = fixture.Create<VerenigingWerdInStopzettingGeplaatst>();
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingWerdInStopzettingGeplaatst,
        };
}
