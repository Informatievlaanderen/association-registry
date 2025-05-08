namespace AssociationRegistry.Test.E2E.When_SubtypeWerdTerugGezetNaarNietBepaald;

using Admin.Api.Verenigingen.Subtype.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.VZER;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(ZetSubtypeNaarNietBepaaldCollection))]
public class ZetSubtypeNaarNietBepaaldCollection : ICollectionFixture<ZetSubtypeNaarNietBepaaldContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class ZetSubtypeNaarNietBepaaldContext : TestContextBase<SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario, WijzigSubtypeRequest>
{
    protected override SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario InitializeScenario()
        => new();

    public ZetSubtypeNaarNietBepaaldContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(SubtypeWerdVerfijndNaarFeitelijkeVerenigingScenario scenario)
    {
        CommandResult = await new WijzigSubtypeRequestVoorNietBepaaldFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
