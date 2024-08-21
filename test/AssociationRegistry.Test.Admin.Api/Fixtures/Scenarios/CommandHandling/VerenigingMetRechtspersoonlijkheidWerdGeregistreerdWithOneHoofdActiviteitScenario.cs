namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;
using Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithOneHoofdActiviteitScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly HoofdactiviteitenVerenigingsloketWerdenGewijzigd HoofdactiviteitenVerenigingsloketWerdenGewijzigd;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithOneHoofdActiviteitScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        HoofdactiviteitenVerenigingsloketWerdenGewijzigd = fixture.Create<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>() with
        {
            HoofdactiviteitenVerenigingsloket = new[] { fixture.Create<Registratiedata.HoofdactiviteitVerenigingsloket>() }
        };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            HoofdactiviteitenVerenigingsloketWerdenGewijzigd,
        };
}
