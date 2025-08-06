namespace AssociationRegistry.Test.E2E.When_Wijzig_Locatie;

using Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Xunit;

[CollectionDefinition(nameof(WijzigLocatieCollection))]
public class WijzigLocatieCollection : ICollectionFixture<WijzigLocatieContext>
{
}

public class WijzigLocatieContext: TestContextBase<FeitelijkeVerenigingWerdGeregistreerdScenario, WijzigLocatieRequest>
{
    protected override FeitelijkeVerenigingWerdGeregistreerdScenario InitializeScenario()
        => new();


    public WijzigLocatieContext(FullBlownApiSetup apiSetup) :  base(apiSetup)
    {
    }


    protected override async ValueTask ExecuteScenario(FeitelijkeVerenigingWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new WijzigLocatieRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
