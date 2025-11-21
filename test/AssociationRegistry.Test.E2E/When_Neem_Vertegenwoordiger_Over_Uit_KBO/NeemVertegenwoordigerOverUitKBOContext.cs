namespace AssociationRegistry.Test.E2E.When_Neem_Vertegenwoordiger_Uit_KBO;

using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class NeemVertegenwoordigerOverUitKBOContext : TestContextBase<VertegenwoordigerWerdOvergenomenUitKBOScenario, NullRequest>
{

    protected override VertegenwoordigerWerdOvergenomenUitKBOScenario InitializeScenario()
        => new();

    public NeemVertegenwoordigerOverUitKBOContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerWerdOvergenomenUitKBOScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
                                                       new NullRequest());
    }
}

[CollectionDefinition(nameof(NeemVertegenwoordigerOverUitKBOCollection))]
public class NeemVertegenwoordigerOverUitKBOCollection : ICollectionFixture<NeemVertegenwoordigerOverUitKBOContext>
{
}
