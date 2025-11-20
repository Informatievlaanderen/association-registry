namespace AssociationRegistry.Test.E2E.When_Wijzig_Vertegenwoordiger_Voor_Rechtspersoon;

using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.WijzigVertegenwoordiger.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.MetRechtspersoonlijkheid;
using Scenarios.Requests.VerenigingMetRechtspersoonlijkheid;
using Xunit;

public class WijzigVertegenwoordigerContextVoorRechtspersoon : TestContextBase<VertegenwoordigerWerdOvergenomenUitKBOScenario, WijzigVertegenwoordigerRequest>
{

    protected override VertegenwoordigerWerdOvergenomenUitKBOScenario InitializeScenario()
        => new();

    public WijzigVertegenwoordigerContextVoorRechtspersoon(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerWerdOvergenomenUitKBOScenario scenario)
    {
        CommandResult = await new WijzigVertegenwoordigerRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(WijzigVertegenwoordigerVoorRechtspersoonCollection))]
public class WijzigVertegenwoordigerVoorRechtspersoonCollection : ICollectionFixture<WijzigVertegenwoordigerContextVoorRechtspersoon>
{
}
