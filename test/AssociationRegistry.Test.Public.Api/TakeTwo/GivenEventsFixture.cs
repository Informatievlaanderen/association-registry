namespace AssociationRegistry.Test.Public.Api.TakeTwo;

using Xunit.Categories;

[Category("PublicApi")]
[IntegrationTest]
public class GivenEventsFixture : PublicApiFixture2
{
    public readonly VerenigingWerdGeregistreerdScenario VerenigingWerdGeregistreerdScenario = new();
    public readonly VerenigingWerdGeregistreerdWithMinimalFieldsScenario VerenigingWerdGeregistreerdWithMinimalFieldsScenario = new();
    public readonly KorteBeschrijvingWerdGewijzigdScenario KorteBeschrijvingWerdGewijzigdScenario = new();

    private IEnumerable<IScenario> Scenarios
        => new IScenario[]
        {
            VerenigingWerdGeregistreerdScenario,
            VerenigingWerdGeregistreerdWithMinimalFieldsScenario,
            KorteBeschrijvingWerdGewijzigdScenario
        };

    public override async Task InitializeAsync()
    {
        foreach (var scenario in Scenarios)
            await AddEvents(scenario.VCode,
                scenario.GetEvents(),
                scenario.GetCommandMetadata());
    }
}

