namespace AssociationRegistry.Test.E2E.When_The_Api_Is_Started;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens;
using Scenarios.Requests;
using When_Messages_Are_Queued;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(ApiStartedCollection))]
public class ApiStartedCollection : ICollectionFixture<MessageQueueContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class ApiStartedContext : TestContextBase<EmptyScenario, NullRequest>
{
    protected override EmptyScenario InitializeScenario()
        => new();

    public ApiStartedContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
        CommandResult = CommandResult<NullRequest>.NullCommandResult();
    }

    protected override async ValueTask ExecuteScenario(EmptyScenario scenario)
    {
    }
}
