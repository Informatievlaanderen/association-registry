namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;
using Vereniging;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithHoofdActiviteitenScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly HoofdactiviteitenVerenigingsloketWerdenGewijzigd HoofdactiviteitenVerenigingsloketWerdenGewijzigd;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithHoofdActiviteitenScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        HoofdactiviteitenVerenigingsloketWerdenGewijzigd = fixture.Create<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>() with
        {
            HoofdactiviteitenVerenigingsloket = fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray(),
        };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            HoofdactiviteitenVerenigingsloketWerdenGewijzigd,
        };
}
