namespace AssociationRegistry.Test.E2E.When_Verwijder_Bankrekeningnummer_Uit_KBO;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests;
using Events;
using Xunit;

public class VerwijderBankrekeningnummerUitKBOContext : TestContextBase<BankrekeningnummerWerdVerwijderdUitKBOScenario, NullRequest>
{

    protected override BankrekeningnummerWerdVerwijderdUitKBOScenario InitializeScenario()
        => new();

    public VerwijderBankrekeningnummerUitKBOContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(BankrekeningnummerWerdVerwijderdUitKBOScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
                                                       new NullRequest());
    }
}

[CollectionDefinition(nameof(BankrekeningnummerWerdVerwijderdUitKBOCollection))]
public class BankrekeningnummerWerdVerwijderdUitKBOCollection : ICollectionFixture<VerwijderBankrekeningnummerUitKBOContext>
{
}
