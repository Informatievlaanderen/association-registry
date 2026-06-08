namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithTeVerlopenErkenningScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerdTeVerlopen;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithTeVerlopenErkenningScenario()
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
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerdTeVerlopen,
        };
}
