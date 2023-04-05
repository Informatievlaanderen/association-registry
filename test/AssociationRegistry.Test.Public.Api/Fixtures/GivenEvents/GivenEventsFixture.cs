namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

public class GivenEventsFixture : PublicApiFixture
{
    public readonly V001_VerenigingWerdGeregistreerdScenario V001VerenigingWerdGeregistreerdScenario = new();
    public readonly V002_VerenigingWerdGeregistreerdWithMinimalFieldsScenario V002VerenigingWerdGeregistreerdWithMinimalFieldsScenario = new();
    public readonly V003_BasisgegevensWerdenGewijzigdScenario V003BasisgegevensWerdenGewijzigdScenario = new();
    public readonly V004_UnHandledEventAndVerenigingWerdGeregistreerdScenario V004UnHandledEventAndVerenigingWerdGeregistreerdScenario = new();
    public readonly V005_ContactgegevenWerdToegevoegdScenario V005ContactgegevenWerdToegevoegdScenario = new();

    private IEnumerable<IScenario> Scenarios
        => new IScenario[]
        {
            V001VerenigingWerdGeregistreerdScenario,
            V002VerenigingWerdGeregistreerdWithMinimalFieldsScenario,
            V003BasisgegevensWerdenGewijzigdScenario,
            V004UnHandledEventAndVerenigingWerdGeregistreerdScenario,
            V005ContactgegevenWerdToegevoegdScenario,
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
