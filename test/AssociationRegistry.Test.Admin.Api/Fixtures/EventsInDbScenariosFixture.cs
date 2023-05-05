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
    public readonly V009_VerenigingWerdGeregistreerd_ForDuplicateForce V009VerenigingWerdGeregistreerdForDuplicateForce = new();
    public readonly V010_VerenigingWerdGeregistreerd_WithAllFields V010VerenigingWerdGeregistreerdWithAllFields = new();
    public readonly V011_VerenigingWerdGeregistreerd_WithVertegenwoordiger_ForRemovingVertegenwoordiger V011VerenigingWerdGeregistreerdWithVertegenwoordigerForRemovingVertegenwoordiger = new();
    public readonly V012_VerenigingWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger V012VerenigingWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger = new();
    public readonly V013_VerenigingWerdGeregistreerd_WithAllFields_ForDuplicateCheck V013VerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck = new();
    public readonly V014_VerenigingWerdGeregistreerd_WithAllFields_ForWijzigBasisgegevens V014VerenigingWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens = new();

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
            V009VerenigingWerdGeregistreerdForDuplicateForce,
            V010VerenigingWerdGeregistreerdWithAllFields,
            V011VerenigingWerdGeregistreerdWithVertegenwoordigerForRemovingVertegenwoordiger,
            V012VerenigingWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger,
            V013VerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck,
            V014VerenigingWerdGeregistreerdWithAllFieldsForWijzigBasisgegevens,
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
