namespace AssociationRegistry.Test.E2E.Erkenningen.When_Verwijder_Erkenning;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class VerwijderErkenningContext : TestContextBase<ErkenningWerdGeregistreerdScenario, NullRequest>
{
    protected override ErkenningWerdGeregistreerdScenario InitializeScenario() => new();

    public VerwijderErkenningContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(ErkenningWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new VerwijderErkenningRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VerwijderErkenningCollection))]
public class VerwijderErkenningCollection : ICollectionFixture<VerwijderErkenningContext> { }
