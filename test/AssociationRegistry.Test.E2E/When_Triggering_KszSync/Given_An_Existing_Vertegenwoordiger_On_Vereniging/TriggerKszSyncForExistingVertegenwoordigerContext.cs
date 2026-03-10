namespace AssociationRegistry.Test.E2E.When_Triggering_KszSync.Given_An_Existing_Vertegenwoordiger_On_Vereniging;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Scenarios.Requests.SuperAdmin;
using Xunit;

public class TriggerKszSyncForExistingVertegenwoordigerContext
    : TestContextBase<VzerWerdGeregistreerdForKszSyncScenario, NullRequest>
{
    protected override VzerWerdGeregistreerdForKszSyncScenario InitializeScenario() => new();

    public TriggerKszSyncForExistingVertegenwoordigerContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(VzerWerdGeregistreerdForKszSyncScenario scenario)
    {
        CommandResult = await new KszSyncRequestFactory(scenario, scenario.Vertegenwoordiger.Insz).ExecuteRequest(
            ApiSetup
        );
    }
}

[CollectionDefinition(nameof(ForExistingVertegenwoordigerCollection))]
public class ForExistingVertegenwoordigerCollection
    : ICollectionFixture<TriggerKszSyncForExistingVertegenwoordigerContext> { }
