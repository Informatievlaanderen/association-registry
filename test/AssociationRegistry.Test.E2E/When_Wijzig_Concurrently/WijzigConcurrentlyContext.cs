namespace AssociationRegistry.Test.E2E.When_Wijzig_Concurrently;

using Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Test.E2E.Scenarios.Requests;
using Framework.TestClasses;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(WijzigConcurrentlyCollection))]
public class WijzigConcurrentlyCollection : ICollectionFixture<WijzigConcurrentlyContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class WijzigConcurrentlyContext : TestContextBase<FeitelijkeVerenigingWerdGeregistreerdScenario, WijzigLocatieRequest>
{
    protected override FeitelijkeVerenigingWerdGeregistreerdScenario InitializeScenario()
        => new();

    public WijzigConcurrentlyContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(FeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new WijzigLocatieRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
