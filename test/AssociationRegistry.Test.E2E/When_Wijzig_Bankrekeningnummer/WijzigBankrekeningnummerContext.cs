namespace AssociationRegistry.Test.E2E.When_Wijzig_Bankrekeningnummer;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.WijzigBankrekeningnummer.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class WijzigBankrekeningnummerContext : TestContextBase<BankrekeningnummerWerdToegevoegdScenario, WijzigBankrekeningnummerRequest>
{

    protected override BankrekeningnummerWerdToegevoegdScenario InitializeScenario()
        => new();

    public WijzigBankrekeningnummerContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(BankrekeningnummerWerdToegevoegdScenario scenario)
    {
        CommandResult = await new WijzigBankrekeningnummerRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(WijzigBankrekeningnummerCollection))]
public class WijzigBankrekeningnummerCollection : ICollectionFixture<WijzigBankrekeningnummerContext>
{
}
