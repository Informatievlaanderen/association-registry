namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using EventStore;
using Scenarios;

public class EventsInDbScenariosFixture : AdminApiFixture
{
    public readonly V001_VerenigingWerdGeregistreerd_WithAllFields V001VerenigingWerdGeregistreerdWithAllFields = new();
    public readonly V002_VerenigingWerdGeregistreerd_WithMinimalFields V002VerenigingWerdGeregistreerdWithMinimalFields = new();
    public readonly V003_VerenigingWerdGeregistreerd_ForUseWithNoChanges V003VerenigingWerdGeregistreerdForUseWithNoChanges = new();
    public readonly V004_AlleBasisGegevensWerdenGewijzigd V004AlleBasisGegevensWerdenGewijzigd = new();
    public readonly V005_VerenigingWerdGeregistreerd_ForUseWithETagMatching V005VerenigingWerdGeregistreerdForUseWithETagMatching = new();
    public readonly V006_ContactgegevenWerdToegevoegd V006ContactgegevenWerdToegevoegd = new();
    public readonly V007_VerenigingWerdGeregistreerd_WithContactgegeven V007VerenigingWerdGeregistreerdWithContactgegeven = new();
    public readonly V008_VerenigingWerdGeregistreerd_WithContactgegeven V008VerenigingWerdGeregistreerdWithContactgegeven = new();

    protected override async Task Given()
    {
        var scenarios = new IEventsInDbScenario[]
        {
            V001VerenigingWerdGeregistreerdWithAllFields,
            V002VerenigingWerdGeregistreerdWithMinimalFields,
            V003VerenigingWerdGeregistreerdForUseWithNoChanges,
            V004AlleBasisGegevensWerdenGewijzigd,
            V005VerenigingWerdGeregistreerdForUseWithETagMatching,
            V006ContactgegevenWerdToegevoegd,
            V007VerenigingWerdGeregistreerdWithContactgegeven,
            V008VerenigingWerdGeregistreerdWithContactgegeven,
        };

        foreach (var scenario in scenarios)
        {
            scenario.Result = await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
        }
    }
}

public class AdminApiScenarioFixture : AdminApiFixture
{
    protected override Task Given()
        => Task.CompletedTask;

    public async Task<StreamActionResult> Apply(IEventsInDbScenario scenario)
        => await AddEvents(scenario.VCode, scenario.GetEvents(), scenario.GetCommandMetadata());
}
