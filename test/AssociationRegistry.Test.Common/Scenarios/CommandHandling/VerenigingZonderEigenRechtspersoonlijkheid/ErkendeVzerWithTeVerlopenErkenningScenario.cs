namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class ErkendeVzerWithTeVerlopenErkenningScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerdTeVerlopen;
    public readonly VerenigingWerdErkend VerenigingWerdErkend;

    public ErkendeVzerWithTeVerlopenErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        var today = DateOnly.FromDateTime(DateTime.Now);
        var einddatum = today.AddDays(-fixture.Create<int>());
        var hernieuwingsdatum = einddatum.AddDays(-fixture.Create<int>());
        var startdatum = hernieuwingsdatum.AddDays(-fixture.Create<int>());

        ErkenningWerdGeregistreerdTeVerlopen = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = startdatum,
            Hernieuwingsdatum = hernieuwingsdatum,
            Einddatum = einddatum,
            Status = ErkenningStatus.Actief.Value,
        };

        VerenigingWerdErkend = fixture.Create<VerenigingWerdErkend>();
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerdTeVerlopen,
            VerenigingWerdErkend,
        };
}
