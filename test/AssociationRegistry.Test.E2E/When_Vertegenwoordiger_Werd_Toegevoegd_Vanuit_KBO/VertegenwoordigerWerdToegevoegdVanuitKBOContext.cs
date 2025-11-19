namespace AssociationRegistry.Test.E2E.When_Vertegenwoordiger_Werd_Toegevoegd_Vanuit_KBO;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.MetRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class VertegenwoordigerWerdToegevoegdVanuitKBOContext : TestContextBase<VertegenwoordigerWerdToegevoegdVanuitKBOScenario, NullRequest>
{

    protected override VertegenwoordigerWerdToegevoegdVanuitKBOScenario InitializeScenario()
        => new();

    public VertegenwoordigerWerdToegevoegdVanuitKBOContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerWerdToegevoegdVanuitKBOScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(scenario.VCode, new NullRequest(), null);
    }
}

[CollectionDefinition(nameof(VertegenwoordigerWerdToegevoegdVanuitKBOCollection))]
public class VertegenwoordigerWerdToegevoegdVanuitKBOCollection : ICollectionFixture<VertegenwoordigerWerdToegevoegdVanuitKBOContext>
{
}
