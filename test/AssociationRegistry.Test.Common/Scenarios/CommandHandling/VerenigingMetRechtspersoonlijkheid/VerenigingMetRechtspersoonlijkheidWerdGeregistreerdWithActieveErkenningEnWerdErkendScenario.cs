namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningEnWerdErkendScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly VerenigingWerdErkend VerenigingWerdErkend;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningEnWerdErkendScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var today = DateOnly.FromDateTime(DateTime.Today);

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = today.AddDays(-10),
            Einddatum = today.AddDays(10),
            Hernieuwingsdatum = today.AddDays(5),
            Status = ErkenningStatus.Actief.Value,
        };

        VerenigingWerdErkend = fixture.Create<VerenigingWerdErkend>();
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerd,
            VerenigingWerdErkend,
        };
}
