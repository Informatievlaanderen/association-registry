namespace AssociationRegistry.Test.E2E.Erkenningen.When_Hef_Schorsings_Erkenning_Op;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class HefSchorsingErkenningOpContext : TestContextBase<ErkenningWerdGeregistreerdScenario, NullRequest>
{
    protected override ErkenningWerdGeregistreerdScenario InitializeScenario() => new();

    public HefSchorsingErkenningOpContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(ErkenningWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new HefSchorsingErkenningOpRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(HefSchorsingErkenningOpCollection))]
public class HefSchorsingErkenningOpCollection : ICollectionFixture<HefSchorsingErkenningOpContext> { }
