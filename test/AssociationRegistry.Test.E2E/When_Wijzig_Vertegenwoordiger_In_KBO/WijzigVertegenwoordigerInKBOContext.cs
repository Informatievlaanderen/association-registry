namespace AssociationRegistry.Test.E2E.When_Wijzig_Vertegenwoordiger_In_KBO;

using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class WijzigVertegenwoordigerInKBOContext : TestContextBase<VertegenwoordigerWerdGewijzigdInKBOScenario, NullRequest>
{

    protected override VertegenwoordigerWerdGewijzigdInKBOScenario InitializeScenario()
        => new();

    public WijzigVertegenwoordigerInKBOContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerWerdGewijzigdInKBOScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
                                                       new NullRequest());
    }
}

[CollectionDefinition(nameof(WijzigVertegenwoordigerInKBOCollection))]
public class WijzigVertegenwoordigerInKBOCollection : ICollectionFixture<WijzigVertegenwoordigerInKBOContext>
{
}
