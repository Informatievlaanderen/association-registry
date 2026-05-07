namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly ErkenningWerdVerlopen ErkenningWerdVerlopen;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>();
        ErkenningWerdVerlopen = fixture.Create<ErkenningWerdVerlopen>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd, ErkenningWerdVerlopen };
}
