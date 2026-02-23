namespace AssociationRegistry.Test.E2E.When_Voeg_Vertegenwoordiger_Toe_After_Ksz_Sync;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using AssociationRegistry.Test.E2E.When_Registreer_Vzer_AfterKszSync;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

public class VoegVertegenwoordigerToeAfterKszSyncContext
    : TestContextBase<VzerWerdGeregistreerdAfterKszSyncScenario, VoegVertegenwoordigerToeRequest>
{
    protected override VzerWerdGeregistreerdAfterKszSyncScenario InitializeScenario() => new();

    public VoegVertegenwoordigerToeAfterKszSyncContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(VzerWerdGeregistreerdAfterKszSyncScenario scenario)
    {
        CommandResult = await new VoegVertegenwoordigerToeAfterKszSyncRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VoegVertegenwoordigerToeAfterKszSyncCollection))]
public class VoegVertegenwoordigerToeAfterKszSyncCollection
    : ICollectionFixture<VoegVertegenwoordigerToeAfterKszSyncContext> { }
