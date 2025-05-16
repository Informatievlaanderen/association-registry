namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van;

using Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(MarkeerAlsDubbelVanCollection))]
public class MarkeerAlsDubbelVanCollection : ICollectionFixture<MarkeerAlsDubbelVanContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class MarkeerAlsDubbelVanContext : TestContextBase<MultipleWerdGeregistreerdScenario, MarkeerAlsDubbelVanRequest>
{
    protected override MultipleWerdGeregistreerdScenario InitializeScenario()
        => new MultipleWerdGeregistreerdScenario();

    public MarkeerAlsDubbelVanContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(MultipleWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new MarkeerAlsDubbelVanRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
