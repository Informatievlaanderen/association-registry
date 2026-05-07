namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly ErkenningWerdVerlopen ErkenningWerdVerlopen;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithVerlopenErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
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
        new IEvent[] { VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd, ErkenningWerdVerlopen };
}
