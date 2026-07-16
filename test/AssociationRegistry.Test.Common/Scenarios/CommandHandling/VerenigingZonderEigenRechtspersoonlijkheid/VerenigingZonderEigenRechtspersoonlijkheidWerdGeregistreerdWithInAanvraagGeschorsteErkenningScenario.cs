namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithInAanvraagGeschorsteErkenningScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly ErkenningWerdGeschorst ErkenningWerdGeschorst;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithInAanvraagGeschorsteErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        var today = DateOnly.FromDateTime(DateTime.Now);

        var startdatum = today.AddDays(fixture.Create<int>());

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = startdatum,
            Einddatum = startdatum.AddDays(fixture.Create<int>()),
        };

        ErkenningWerdGeschorst = fixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerd,
            ErkenningWerdGeschorst,
        };
}
