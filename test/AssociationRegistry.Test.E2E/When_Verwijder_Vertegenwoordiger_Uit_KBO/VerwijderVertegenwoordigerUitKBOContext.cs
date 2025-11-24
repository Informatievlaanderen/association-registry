namespace AssociationRegistry.Test.E2E.When_Verwijder_Vertegenwoordiger_Uit_KBO;

using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class VerwijderVertegenwoordigerUitKBOContext : TestContextBase<VertegenwoordigerWerdVerwijderdUitKBOScenario, NullRequest>
{

    protected override VertegenwoordigerWerdVerwijderdUitKBOScenario InitializeScenario()
        => new();

    public VerwijderVertegenwoordigerUitKBOContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerWerdVerwijderdUitKBOScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
                                                       new NullRequest());
    }
}

[CollectionDefinition(nameof(VerwijderVertegenwoordigerUitKBOCollection))]
public class VerwijderVertegenwoordigerUitKBOCollection : ICollectionFixture<VerwijderVertegenwoordigerUitKBOContext>
{
}
