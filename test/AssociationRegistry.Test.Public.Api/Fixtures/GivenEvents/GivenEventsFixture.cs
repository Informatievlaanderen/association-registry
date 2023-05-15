namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

public class GivenEventsFixture : PublicApiFixture
{
    public readonly V001_FeitelijkeVerenigingWerdGeregistreerdScenario V001FeitelijkeVerenigingWerdGeregistreerdScenario = new();
    public readonly V002_FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario = new();
    public readonly V003_BasisgegevensWerdenGewijzigdScenario V003BasisgegevensWerdenGewijzigdScenario = new();
    public readonly V004_UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario V004UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario = new();
    public readonly V005_ContactgegevenWerdToegevoegdScenario V005ContactgegevenWerdToegevoegdScenario = new();

    private IEnumerable<IScenario> Scenarios
        => new IScenario[]
        {
            V001FeitelijkeVerenigingWerdGeregistreerdScenario,
            V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsScenario,
            V003BasisgegevensWerdenGewijzigdScenario,
            V004UnHandledEventAndFeitelijkeVerenigingWerdGeregistreerdScenario,
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
