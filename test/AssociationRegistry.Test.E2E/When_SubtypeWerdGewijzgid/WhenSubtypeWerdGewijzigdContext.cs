namespace AssociationRegistry.Test.E2E.When_SubtypeWerdGewijzgid;

using Admin.Api.WebApi.Verenigingen.Subtype.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.VZER;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(WhenSubtypeWerdGewijzigdCollection))]
public class WhenSubtypeWerdGewijzigdCollection : ICollectionFixture<WhenSubtypeWerdGewijzigdContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class WhenSubtypeWerdGewijzigdContext : TestContextBase<SubtypeWerdVerfijndNaarSubverenigingScenario, WijzigSubtypeRequest>
{
    protected override SubtypeWerdVerfijndNaarSubverenigingScenario InitializeScenario()
        => new();

    public WhenSubtypeWerdGewijzigdContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(SubtypeWerdVerfijndNaarSubverenigingScenario scenario)
    {
        CommandResult = await new WijzigSubtypeRequestVoorWijzigSubtypeFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
