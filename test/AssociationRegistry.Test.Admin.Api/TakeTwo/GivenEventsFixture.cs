namespace AssociationRegistry.Test.Admin.Api.TakeTwo;

public class GivenEventsFixture : AdminApiFixture2
{
    public readonly VerenigingWerdGeregistreerd_WithAllFields_Scenario VerenigingWerdGeregistreerdWithAllFieldsScenario = new();
    public readonly VerenigingWerdGeregistreerd_WithMinimalFields_Scenario VerenigingWerdGeregistreerdWithMinimalFieldsScenario = new();
    public readonly AlleBasisGegevensWerdenGewijzigd_Scenario AlleBasisGegevensWerdenGewijzigdScenario = new();

    protected override async Task Given()
    {
        var scenarios = new IScenario[] { VerenigingWerdGeregistreerdWithAllFieldsScenario, VerenigingWerdGeregistreerdWithMinimalFieldsScenario, AlleBasisGegevensWerdenGewijzigdScenario };

        foreach (var scenario in scenarios)
        {
            scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
        }
    }
}
