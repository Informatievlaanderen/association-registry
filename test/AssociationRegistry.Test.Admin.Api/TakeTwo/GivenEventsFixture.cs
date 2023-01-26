namespace AssociationRegistry.Test.Admin.Api.TakeTwo;

public class GivenEventsFixture : AdminApiFixture2
{
    public readonly VerenigingWerdGeregistreerdScenario VerenigingWerdGeregistreerdScenario = new();
    public readonly VerenigingWerdGeregistreerdWithMinimalFieldsScenario VerenigingWerdGeregistreerdWithMinimalFieldsScenario = new();
    public readonly AlleBasisGegevensWerdenGewijzigdScenario AlleBasisGegevensWerdenGewijzigdScenario = new();

    protected override async Task Given()
    {
        var scenarios = new IScenario[] { VerenigingWerdGeregistreerdScenario, VerenigingWerdGeregistreerdWithMinimalFieldsScenario, AlleBasisGegevensWerdenGewijzigdScenario };

        foreach (var scenario in scenarios)
        {
            scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
        }
    }
}
