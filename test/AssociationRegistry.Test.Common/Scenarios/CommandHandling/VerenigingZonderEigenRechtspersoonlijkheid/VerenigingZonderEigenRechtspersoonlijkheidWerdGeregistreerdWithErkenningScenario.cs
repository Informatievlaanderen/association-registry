namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        var today = DateOnly.FromDateTime(DateTime.Today);
        var startdatum = today.AddDays(-fixture.Create<int>());
        var hernieuwingsdatum = today.AddDays(fixture.Create<int>());
        var einddatum = hernieuwingsdatum.AddDays(fixture.Create<int>());

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = startdatum,
            Hernieuwingsdatum = hernieuwingsdatum,
            Einddatum = einddatum,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd };
}
