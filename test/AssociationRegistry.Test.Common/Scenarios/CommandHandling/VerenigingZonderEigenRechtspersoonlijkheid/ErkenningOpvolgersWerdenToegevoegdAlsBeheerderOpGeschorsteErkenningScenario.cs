namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpGeschorsteErkenningScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly ErkenningWerdGeschorst ErkenningWerdGeschorst;
    public readonly ErkenningOpvolgersWerdenToegevoegdAlsBeheerder ErkenningOpvolgersWerdenToegevoegdAlsBeheerder;

    public ErkenningOpvolgersWerdenToegevoegdAlsBeheerderOpGeschorsteErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        var today = DateOnly.FromDateTime(DateTime.Now);

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = today.AddDays(-fixture.Create<int>()),
            Einddatum = today.AddDays(fixture.Create<int>()),
        };

        ErkenningWerdGeschorst = fixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };
        ErkenningOpvolgersWerdenToegevoegdAlsBeheerder =
            fixture.Create<ErkenningOpvolgersWerdenToegevoegdAlsBeheerder>() with
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
            ErkenningOpvolgersWerdenToegevoegdAlsBeheerder,
        };
}
