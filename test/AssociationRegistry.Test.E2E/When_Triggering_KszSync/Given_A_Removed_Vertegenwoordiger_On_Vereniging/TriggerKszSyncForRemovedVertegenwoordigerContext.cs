namespace AssociationRegistry.Test.E2E.When_Triggering_KszSync.Given_A_Removed_Vertegenwoordiger_On_Vereniging;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Scenarios.Requests.SuperAdmin;
using Xunit;

public class TriggerKszSyncForRemovedVertegenwoordigerContext
    : TestContextBase<VzerWerdGeregistreerdForKszSyncScenario, NullRequest>
{
    protected override VzerWerdGeregistreerdForKszSyncScenario InitializeScenario() => new();

    public TriggerKszSyncForRemovedVertegenwoordigerContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(VzerWerdGeregistreerdForKszSyncScenario scenario)
    {
        CommandResult = await new KszSyncRequestFactory(
            scenario,
            scenario.VerwijderdeVertegenwoordiger.Insz
        ).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(ForRemovedVertegenwoordigerCollection))]
public class ForRemovedVertegenwoordigerCollection
    : ICollectionFixture<TriggerKszSyncForRemovedVertegenwoordigerContext> { }
