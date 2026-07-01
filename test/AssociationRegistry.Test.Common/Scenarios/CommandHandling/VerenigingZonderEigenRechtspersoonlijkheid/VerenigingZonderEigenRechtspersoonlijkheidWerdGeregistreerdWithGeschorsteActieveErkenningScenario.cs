namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;

using global::AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;

public class VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteActieveErkenningScenario
    : CommandhandlerScenarioBase
{
    public override VCode VCode => VCode.Create("V0009002");

    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd;
    public readonly ErkenningWerdGeschorst ErkenningWerdGeschorst;
    public readonly VerenigingWerdNietLangerErkend VerenigingWerdNietLangerErkend;

    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteActieveErkenningScenario()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
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
            Status = ErkenningStatus.Actief.Value,
        };

        ErkenningWerdGeschorst = fixture.Create<ErkenningWerdGeschorst>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
        };

        VerenigingWerdNietLangerErkend = new VerenigingWerdNietLangerErkend();
    }

    public override IEnumerable<IEvent> Events() =>
        new IEvent[]
        {
            VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd,
            ErkenningWerdGeregistreerd,
            ErkenningWerdGeschorst,
            VerenigingWerdNietLangerErkend,
        };
}
