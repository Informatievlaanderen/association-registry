namespace AssociationRegistry.Test.E2E.When_Registreer_Vzer_AfterKszSync;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens;
using AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(vzerWerdGeregistreerdAfterKszSyncCollection))]
public class vzerWerdGeregistreerdAfterKszSyncCollection : ICollectionFixture<vzerWerdGeregistreerdAfterKszSyncContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class vzerWerdGeregistreerdAfterKszSyncContext
    : TestContextBase<
        VzerWerdGeregistreerdAfterKszSyncScenario,
        RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
    >
{
    protected override VzerWerdGeregistreerdAfterKszSyncScenario InitializeScenario() => new();

    public vzerWerdGeregistreerdAfterKszSyncContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(VzerWerdGeregistreerdAfterKszSyncScenario scenario)
    {
        CommandResult = await new RegistreerVZERRequestFactory().ExecuteRequest(ApiSetup);
    }
}
