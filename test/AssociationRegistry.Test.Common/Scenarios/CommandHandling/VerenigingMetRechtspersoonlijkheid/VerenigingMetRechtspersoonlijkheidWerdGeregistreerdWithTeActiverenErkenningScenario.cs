namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithTeActiverenErkenningScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithTeActiverenErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        var today = DateOnly.FromDateTime(DateTime.Today);
        var startdatum = today.AddDays(-fixture.Create<int>() - 1);
        var einddatum = today.AddDays(fixture.Create<int>() + 1);
        var hernieuwingsdatum = startdatum.AddDays((einddatum.DayNumber - startdatum.DayNumber) / 2);

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = startdatum,
            Hernieuwingsdatum = hernieuwingsdatum,
            Einddatum = einddatum,
            Status = ErkenningStatus.InAanvraag.Value,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd };
}
