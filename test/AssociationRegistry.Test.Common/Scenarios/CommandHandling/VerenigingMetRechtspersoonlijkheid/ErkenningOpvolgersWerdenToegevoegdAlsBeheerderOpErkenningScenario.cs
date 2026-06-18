namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;

public class ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly ErkenningOpvolgersWerdenToegevoegdAlsBeheerder ErkenningOpvolgersWerdenToegevoegdAlsBeheerder;

    public ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>();
        ErkenningOpvolgersWerdenToegevoegdAlsBeheerder =
            fixture.Create<ErkenningOpvolgersWerdenToegevoegdAlsBeheerder>() with
            {
                ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
            };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerd,
            ErkenningOpvolgersWerdenToegevoegdAlsBeheerder,
        };
}
