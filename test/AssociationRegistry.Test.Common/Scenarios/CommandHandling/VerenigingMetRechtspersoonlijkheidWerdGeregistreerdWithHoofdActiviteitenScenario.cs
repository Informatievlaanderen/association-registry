namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using global::AutoFixture;

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
