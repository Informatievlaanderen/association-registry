namespace AssociationRegistry.Test.E2E.When_Verwijder_Vertegenwoordiger;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Scenarios.Requests;
using Xunit;

public class VerwijderVertegenwoordigerContext : TestContextBase<VertegenwoordigerWerdToegevoegdScenario, NullRequest>
{

    protected override VertegenwoordigerWerdToegevoegdScenario InitializeScenario()
        => new();

    public VerwijderVertegenwoordigerContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerWerdToegevoegdScenario scenario)
    {
        CommandResult = await new VerwijderVertegenwoordigerRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VerwijderVertegenwoordigerCollection))]
public class VerwijderVertegenwoordigerCollection : ICollectionFixture<VerwijderVertegenwoordigerContext>
{
}
