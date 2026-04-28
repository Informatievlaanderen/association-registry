namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly ErkenningWerdGeschorst ErkenningWerdGeschorst;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>();
        ErkenningWerdGeschorst = fixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd, ErkenningWerdGeschorst };
}
