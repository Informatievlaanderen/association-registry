namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario : CommandhandlerScenarioBase
{
    public override VCode VCode { get; }
    public  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; }
    public  VerenigingWerdGestopt VerenigingWerdGestopt { get; }


    public VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = fixture.Create<VCode>();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        VerenigingWerdGestopt = fixture.Create<VerenigingWerdGestopt>();
    }


    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            VerenigingWerdGestopt,
        };
    }
}
