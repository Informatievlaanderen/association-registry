namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using When_Wijzig_Lidmaatschap;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(StopVerenigingCollection))]
public class StopVerenigingCollection : ICollectionFixture<StopVerenigingContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class StopVerenigingContext : TestContextBase<FeitelijkeVerenigingWerdGeregistreerdScenario, StopVerenigingRequest>
{
    protected override FeitelijkeVerenigingWerdGeregistreerdScenario InitializeScenario()
        => new();

    public StopVerenigingContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(FeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new StopVerenigingRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
