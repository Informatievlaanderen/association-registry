namespace AssociationRegistry.Test.E2E.When_Voeg_Bankrekeningnummer_Toe_Vanuit_KBO;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests;
using Xunit;

public class VoegBankrekeningnummerToeVanuitKBOContext : TestContextBase<BankrekeningnummerWerdToegevoegdVanuitKBOScenario, NullRequest>
{

    protected override BankrekeningnummerWerdToegevoegdVanuitKBOScenario InitializeScenario()
        => new();

    public VoegBankrekeningnummerToeVanuitKBOContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(BankrekeningnummerWerdToegevoegdVanuitKBOScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
                                                       new NullRequest());
    }
}

[CollectionDefinition(nameof(VoegBankrekeningnummerToeVanuitKBOCollection))]
public class VoegBankrekeningnummerToeVanuitKBOCollection : ICollectionFixture<VoegBankrekeningnummerToeVanuitKBOContext>
{
}
