namespace AssociationRegistry.Test.E2E.When_Valideer_Bankrekeningnummer;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class ValideerBankrekeningnummerContext : TestContextBase<BankrekeningnummerWerdToegevoegdScenario, NullRequest>
{

    protected override BankrekeningnummerWerdToegevoegdScenario InitializeScenario()
        => new();

    public ValideerBankrekeningnummerContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(BankrekeningnummerWerdToegevoegdScenario scenario)
    {
        CommandResult = await new ValideerBankrekeningnummerRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(ValideerBankrekeningnummerCollection))]
public class ValideerBankrekeningnummerCollection : ICollectionFixture<ValideerBankrekeningnummerContext>
{
}
