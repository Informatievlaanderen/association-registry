namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithMultipleActieveErkenningenEnWerdErkendScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ActieveErkenningEen;
    public readonly ErkenningWerdGeregistreerd ActieveErkenningTwee;
    public readonly VerenigingWerdErkend VerenigingWerdErkend;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithMultipleActieveErkenningenEnWerdErkendScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var today = DateOnly.FromDateTime(DateTime.Today);

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ActieveErkenningEen = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = today.AddDays(-10),
            Einddatum = today.AddDays(10),
            Hernieuwingsdatum = today.AddDays(5),
            Status = ErkenningStatus.Actief.Value,
        };

        ActieveErkenningTwee = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = today.AddDays(-5),
            Einddatum = today.AddDays(15),
            Hernieuwingsdatum = today.AddDays(10),
            Status = ErkenningStatus.Actief.Value,
        };

        VerenigingWerdErkend = fixture.Create<VerenigingWerdErkend>();
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ActieveErkenningEen,
            ActieveErkenningTwee,
            VerenigingWerdErkend,
        };
}
