namespace AssociationRegistry.Test.E2E.When_Aanvaard_Dubbel_Van_Handler_Fails;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens;
using Scenarios.Requests;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(AanvaardDubbelHandlerCollection))]
public class AanvaardDubbelHandlerCollection : ICollectionFixture<AanvaardDubbelHandlerContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class AanvaardDubbelHandlerContext : TestContextBase<EmptyScenario, NullRequest>
{
    protected override EmptyScenario InitializeScenario()
        => new();

    public AanvaardDubbelHandlerContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
        CommandResult = CommandResult<NullRequest>.NullCommandResult();
    }

    protected override async ValueTask ExecuteScenario(EmptyScenario scenario)
    {
    }
}
