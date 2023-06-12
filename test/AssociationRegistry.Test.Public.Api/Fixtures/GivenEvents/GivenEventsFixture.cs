namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

using Scenarios;

public class GivenEventsFixture : PublicApiFixture
{
    public readonly V001_FeitelijkeVerenigingWerdGeregistreerdScenario V001FeitelijkeVerenigingWerdGeregistreerdScenario = new();
    public readonly V002_FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario = new();
    public readonly V003_BasisgegevensWerdenGewijzigdScenario V003BasisgegevensWerdenGewijzigdScenario = new();
    public readonly V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario V004UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario = new();
    public readonly V005_ContactgegevenWerdToegevoegdScenario V005ContactgegevenWerdToegevoegdScenario = new();
    public readonly V006_VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario V006VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario = new();
    public readonly V007_AfdelingWerdGeregistreerdScenario V007AfdelingWerdGeregistreerdScenario = new();
    public readonly V009_MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario V009MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario = new();
    public readonly V010_FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario V010FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario = new();

    private IEnumerable<IScenario> Scenarios
        => new IScenario[]
        {
            V001FeitelijkeVerenigingWerdGeregistreerdScenario,
            V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario,
            V003BasisgegevensWerdenGewijzigdScenario,
            V004UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario,
            V005ContactgegevenWerdToegevoegdScenario,
            V006VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario,
            V007AfdelingWerdGeregistreerdScenario,
            V009MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario,
            V010FeitelijkeVerenigingWerdGeregistreerdAndUitgeschrevenUitPubliekeDatastroomScenario,
        };

    public override async Task InitializeAsync()
    {
        foreach (var scenario in Scenarios)
            await AddEvents(
                scenario.VCode,
                scenario.GetEvents(),
                scenario.GetCommandMetadata());
    }
}
