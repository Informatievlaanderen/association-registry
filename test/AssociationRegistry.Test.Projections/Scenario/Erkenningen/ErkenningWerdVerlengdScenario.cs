namespace AssociationRegistry.Test.Projections.Scenario.Erkenningen;

using Events;
using AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;

public class ErkenningWerdVerlengdScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd { get; set; }

    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public ErkenningWerdVerlengd ErkenningWerdVerlengd { get; }

    public ErkenningWerdVerlengdScenario()
    {
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>();
        var verlengdeHernieuwingsdatum = ErkenningWerdGeregistreerd.Einddatum.Value.AddDays(AutoFixture.Create<int>());
        var verlengdeEinddatum = verlengdeHernieuwingsdatum.AddDays(AutoFixture.Create<int>());
        var today = DateOnly.FromDateTime(DateTime.Today);
        var status = ErkenningStatus.Bepaal(
            ErkenningsPeriode.Create(ErkenningWerdGeregistreerd.Startdatum, verlengdeEinddatum), today);

        ErkenningWerdVerlengd = AutoFixture.Create<ErkenningWerdVerlengd>() with
        {
            ErkenningId = ErkenningWerdGeregistreerd.ErkenningId,
            Einddatum = verlengdeEinddatum,
            Hernieuwingsdatum = verlengdeEinddatum,
            Status = status.Value,
        };
    }

    public override string AggregateId => VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
    [
        new(AggregateId, VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd, ErkenningWerdGeregistreerd, ErkenningWerdVerlengd),
    ];
}
