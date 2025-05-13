namespace AssociationRegistry.Test.E2E.When_Wijzig_Locatie;

using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging;
using Marten.Events;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using When_Wijzig_Lidmaatschap;
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
