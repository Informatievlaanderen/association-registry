namespace AssociationRegistry.Test.E2E.When_Verwijder_Bankrekeningnummer;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using Scenarios.Requests;
using Xunit;

public class VerwijderBankrekeningnummerContext : TestContextBase<BankrekeningnummerWerdToegevoegdScenario, NullRequest>
{

    protected override BankrekeningnummerWerdToegevoegdScenario InitializeScenario()
        => new();

    public VerwijderBankrekeningnummerContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(BankrekeningnummerWerdToegevoegdScenario scenario)
    {
        CommandResult = await new VerwijderBankrekeningnummerRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VerwijderBankrekeningnummerCollection))]
public class VerwijderBankrekeningnummerCollection : ICollectionFixture<VerwijderBankrekeningnummerContext>
{
}
