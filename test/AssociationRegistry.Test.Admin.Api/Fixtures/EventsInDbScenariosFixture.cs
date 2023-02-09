namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using Scenarios;

public class EventsInDbScenariosFixture : AdminApiFixture
{
    public readonly VerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario = new();
    public readonly VerenigingWerdGeregistreerd_WithMinimalFields_EventsInDbScenario VerenigingWerdGeregistreerdWithMinimalFieldsEventsInDbScenario = new();
    public readonly VerenigingWerdGeregistreerd_ForUseWithNoChanges_EventsInDbScenario VerenigingWerdGeregistreerdForUseWithNoChangesEventsInDbScenario = new();
    public readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario AlleBasisGegevensWerdenGewijzigdEventsInDbScenario = new();
    public readonly VerenigingWerdGeregistreerd_ForUseWithETagMatching_EventsInDbScenario VerenigingWerdGeregistreerdForUseWithETagMatchingEventsInDbScenario = new();

    protected override async Task Given()
    {
        var scenarios = new IEventsInDbScenario[]
        {
            VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario,
            VerenigingWerdGeregistreerdWithMinimalFieldsEventsInDbScenario,
            VerenigingWerdGeregistreerdForUseWithNoChangesEventsInDbScenario,
            AlleBasisGegevensWerdenGewijzigdEventsInDbScenario,
            VerenigingWerdGeregistreerdForUseWithETagMatchingEventsInDbScenario,
        };

        foreach (var scenario in scenarios)
        {
            scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
        }
    }
}
