namespace AssociationRegistry.Test.E2E.When_Searching;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(SearchCollection))]
public class SearchCollection : ICollectionFixture<SearchContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class SearchContext : TestContextBase<SearchScenario, NullRequest>
{
    protected override SearchScenario InitializeScenario()
        => new();

    public SearchContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(SearchScenario scenario)
    {
        CommandResult = CommandResult<NullRequest>.NullCommandResult();
    }
}
