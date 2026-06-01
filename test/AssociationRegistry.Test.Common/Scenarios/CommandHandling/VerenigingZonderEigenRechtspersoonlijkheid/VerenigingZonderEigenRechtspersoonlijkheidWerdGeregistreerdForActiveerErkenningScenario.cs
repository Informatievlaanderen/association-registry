namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForActiveerErkenningScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForActiveerErkenningScenario(
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

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
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
        new IEvent[] { VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd };
}
