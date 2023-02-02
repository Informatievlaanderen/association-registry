namespace AssociationRegistry.Test.Admin.Api.TakeTwo;

public class EventsInDbScenariosFixture : AdminApiFixture2
{
    public readonly VerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario = new();
    public readonly VerenigingWerdGeregistreerd_WithMinimalFields_EventsInDbScenario VerenigingWerdGeregistreerdWithMinimalFieldsEventsInDbScenario = new();
    public readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario AlleBasisGegevensWerdenGewijzigdEventsInDbScenario = new();

    protected override async Task Given()
    {
        var scenarios = new IEventsInDbScenario[] { VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario, VerenigingWerdGeregistreerdWithMinimalFieldsEventsInDbScenario, AlleBasisGegevensWerdenGewijzigdEventsInDbScenario };

        foreach (var scenario in scenarios)
        {
            scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
        }
    }
}
