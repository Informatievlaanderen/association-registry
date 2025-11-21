namespace AssociationRegistry.Test.E2E.Voeg_Vertegenwoordiger_Toe_Vanuit_KBO;

using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class VoegVertegenwoordigerToeVanuitKBOContext : TestContextBase<VertegenwoordigerWerdToegevoegdVanuitKBOScenario, NullRequest>
{

    protected override VertegenwoordigerWerdToegevoegdVanuitKBOScenario InitializeScenario()
        => new();

    public VoegVertegenwoordigerToeVanuitKBOContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerWerdToegevoegdVanuitKBOScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
                                                       new NullRequest());
    }
}

[CollectionDefinition(nameof(VoegVertegenwoordigerToeVanuitKBOCollection))]
public class VoegVertegenwoordigerToeVanuitKBOCollection : ICollectionFixture<VoegVertegenwoordigerToeVanuitKBOContext>
{
}
