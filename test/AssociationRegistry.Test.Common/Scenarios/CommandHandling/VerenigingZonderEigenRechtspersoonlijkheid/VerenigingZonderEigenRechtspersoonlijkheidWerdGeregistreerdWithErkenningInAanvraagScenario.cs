namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;
using global::AutoFixture;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningInAanvraagScenario : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningInAanvraagScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var today = DateOnly.FromDateTime(DateTime.Today);
        var futureStart = today.AddDays(fixture.Create<int>() + 1);


        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = futureStart,
            Hernieuwingsdatum = futureStart.AddDays(10),
            Einddatum = futureStart.AddDays(20),
            Status =  ErkenningStatus.InAanvraag,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd };
}
