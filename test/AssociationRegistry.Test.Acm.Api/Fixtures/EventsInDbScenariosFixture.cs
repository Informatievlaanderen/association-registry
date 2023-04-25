namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using Scenarios;

public class EventsInDbScenariosFixture : AcmApiFixture
{
    public readonly VerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario = new();
    public readonly VertegenwoordigerWerdToegevoegd_EventsInDbScenario VertegenwoordigerWerdToegevoegdEventsInDbScenario = new();
    public readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario AlleBasisGegevensWerdenGewijzigdEventsInDbScenario = new();

    protected override async Task Given()
    {
        var scenarios = new IEventsInDbScenario[]
        {
            VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario,
            VertegenwoordigerWerdToegevoegdEventsInDbScenario,
            AlleBasisGegevensWerdenGewijzigdEventsInDbScenario,
        };

        foreach (var scenario in scenarios)
        {

                scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
        }
    }
}
