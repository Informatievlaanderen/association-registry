namespace AssociationRegistry.Test.E2E.When_Wijzig_Vertegenwoordiger;

using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class WijzigVertegenwoordigerContext : TestContextBase<VertegenwoordigerWerdToegevoegdScenario, WijzigVertegenwoordigerRequest>
{

    protected override VertegenwoordigerWerdToegevoegdScenario InitializeScenario()
        => new();

    public WijzigVertegenwoordigerContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerWerdToegevoegdScenario scenario)
    {
        CommandResult = await new WijzigVertegenwoordigerRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(WijzigVertegenwoordigerCollection))]
public class WijzigVertegenwoordigerCollection : ICollectionFixture<WijzigVertegenwoordigerContext>
{
}
