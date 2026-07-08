namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };
    }

    public readonly List<IEvent> additionalEvents = new();

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd }.Concat(additionalEvents);
}
