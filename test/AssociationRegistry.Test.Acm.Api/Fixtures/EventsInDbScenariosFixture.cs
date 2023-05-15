namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using Scenarios;

public class EventsInDbScenariosFixture : AcmApiFixture
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd_WithAllFields_EventsInDbScenario FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario = new();
    public readonly VertegenwoordigerWerdToegevoegd_EventsInDbScenario VertegenwoordigerWerdToegevoegdEventsInDbScenario = new();
    public readonly NaamWerdGewijzigd_And_VertegenwoordigerWerdToegevoegd_EventsInDbScenario NaamWerdGewijzigdAndVertegenwoordigerWerdToegevoegdEventsInDbScenario = new();
    public readonly AlleBasisGegevensWerdenGewijzigd_EventsInDbScenario AlleBasisGegevensWerdenGewijzigdEventsInDbScenario = new();
    public readonly VertegenwoordigerWerdVerwijderd_EventsInDbScenario VertegenwoordigerWerdVerwijderdEventsInDbScenario = new();

    protected override async Task Given()
    {
        var scenarios = new IEventsInDbScenario[]
        {
            FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario,
            VertegenwoordigerWerdToegevoegdEventsInDbScenario,
            AlleBasisGegevensWerdenGewijzigdEventsInDbScenario,
            NaamWerdGewijzigdAndVertegenwoordigerWerdToegevoegdEventsInDbScenario,
            VertegenwoordigerWerdVerwijderdEventsInDbScenario,
        };

        foreach (var scenario in scenarios)
        {

                scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
        }
    }
}
