namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

public class GivenEventsFixture : PublicApiFixture
{
    public readonly VerenigingWerdGeregistreerdScenario VerenigingWerdGeregistreerdScenario = new();
    public readonly VerenigingWerdGeregistreerdWithMinimalFieldsScenario VerenigingWerdGeregistreerdWithMinimalFieldsScenario = new();
    public readonly KorteBeschrijvingWerdGewijzigdScenario KorteBeschrijvingWerdGewijzigdScenario = new();
    public readonly UnHandledEventAndVerenigingWerdGeregistreerdScenario UnHandledEventAndVerenigingWerdGeregistreerdScenario = new();

    private IEnumerable<IScenario> Scenarios
        => new IScenario[]
        {
            VerenigingWerdGeregistreerdScenario,
            VerenigingWerdGeregistreerdWithMinimalFieldsScenario,
            KorteBeschrijvingWerdGewijzigdScenario,
            UnHandledEventAndVerenigingWerdGeregistreerdScenario,
        };

    public override async Task InitializeAsync()
    {
        foreach (var scenario in Scenarios)
            await AddEvents(scenario.VCode,
                scenario.GetEvents(),
                scenario.GetCommandMetadata());
    }
}

