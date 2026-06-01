namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForActiveerErkenningScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerdForActiveerErkenningScenario(
        DateOnly? startdatum = null,
        DateOnly? einddatum = null,
        string? status = null
    )
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var today = DateOnly.FromDateTime(DateTime.Today);
        startdatum ??= today;
        einddatum ??= today.AddYears(1);
        status ??= ErkenningStatus.InAanvraag.Value;

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = VCode,
            };

        ErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Startdatum = startdatum,
            Hernieuwingsdatum = startdatum.Value.AddDays(10),
            Einddatum = einddatum,
            Status = status,
        };
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[] { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd };
}
