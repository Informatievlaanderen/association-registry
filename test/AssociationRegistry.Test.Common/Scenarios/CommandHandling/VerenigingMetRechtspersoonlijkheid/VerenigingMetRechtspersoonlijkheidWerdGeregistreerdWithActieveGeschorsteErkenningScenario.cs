namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithActieveGeschorsteErkenningScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly ErkenningWerdGeschorst ErkenningWerdGeschorst;
    public readonly VerenigingWerdErkend VerenigingWerdErkend;
    public readonly VerenigingWerdNietLangerErkend VerenigingWerdNietLangerErkend;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithActieveGeschorsteErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        var today = DateOnly.FromDateTime(DateTime.Now);

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = today.AddDays(-fixture.Create<int>()),
            Einddatum = today.AddDays(fixture.Create<int>()),
        };

        VerenigingWerdErkend = fixture.Create<VerenigingWerdErkend>();

        ErkenningWerdGeschorst = fixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };

        VerenigingWerdNietLangerErkend = fixture.Create<VerenigingWerdNietLangerErkend>();
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerd,
            VerenigingWerdErkend,
            ErkenningWerdGeschorst,
            VerenigingWerdNietLangerErkend,
        };
}
