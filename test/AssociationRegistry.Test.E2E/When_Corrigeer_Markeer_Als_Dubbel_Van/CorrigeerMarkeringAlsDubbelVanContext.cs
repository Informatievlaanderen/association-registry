namespace AssociationRegistry.Test.E2E.When_Corrigeer_Markeer_Als_Dubbel_Van;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(CorrigeerMarkeringAlsDubbelVanCollection))]
public class CorrigeerMarkeringAlsDubbelVanCollection : ICollectionFixture<CorrigeerMarkeringAlsDubbelVanContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class CorrigeerMarkeringAlsDubbelVanContext : TestContextBase<VerenigingWerdGemarkeerdAlsDubbelVanScenario, NullRequest>
{
    protected override VerenigingWerdGemarkeerdAlsDubbelVanScenario InitializeScenario()
        => new();

    public CorrigeerMarkeringAlsDubbelVanContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingWerdGemarkeerdAlsDubbelVanScenario scenario)
    {
        CommandResult = await new CorrigeerMarkeringAlsDubbelVanRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
