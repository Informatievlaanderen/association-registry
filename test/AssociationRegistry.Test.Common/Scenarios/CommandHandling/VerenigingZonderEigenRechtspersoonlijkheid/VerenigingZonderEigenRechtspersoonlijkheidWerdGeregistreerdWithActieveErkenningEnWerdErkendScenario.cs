namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningEnWerdErkendScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly VerenigingWerdErkend VerenigingWerdErkend;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningEnWerdErkendScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var today = DateOnly.FromDateTime(DateTime.Today);

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
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
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerd,
            VerenigingWerdErkend,
        };
}
