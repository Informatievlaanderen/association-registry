namespace AssociationRegistry.Test.E2E.When_Voeg_Bankrekeningnummer_Toe;

using Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VoegBankrekeningnummerToe.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class VoegBankrekeningnummerToeContext : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario, VoegBankrekeningnummerToeRequest>
{

    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario()
        => new();

    public VoegBankrekeningnummerToeContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new VoegBankrekeningnummerToeRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VoegBankrekeningnummerToeCollection))]
public class VoegBankrekeningnummerToeCollection : ICollectionFixture<VoegBankrekeningnummerToeContext>
{
}
