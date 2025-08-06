namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens;

using Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(WijzigBasisGegevensCollection))]
public class WijzigBasisGegevensCollection : ICollectionFixture<WijzigBasisgegevensContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class WijzigBasisgegevensContext : TestContextBase<FeitelijkeVerenigingWerdGeregistreerdScenario, WijzigBasisgegevensRequest>
{
    protected override FeitelijkeVerenigingWerdGeregistreerdScenario InitializeScenario()
        => new(true);

    public WijzigBasisgegevensContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(FeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new WijzigBasisgegevensRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

