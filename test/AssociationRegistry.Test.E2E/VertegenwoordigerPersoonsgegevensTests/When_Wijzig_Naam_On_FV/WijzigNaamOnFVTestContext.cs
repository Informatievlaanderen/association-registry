namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_FV;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using Scenarios.Givens.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevens;
using Scenarios.Requests.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevensTests;
using Xunit;

[CollectionDefinition(nameof(WijzigNaamOnFVTestCollection))]
public class WijzigNaamOnFVTestCollection : ICollectionFixture<WijzigNaamOnFVTestContext>
{
}

public class WijzigNaamOnFVTestContext : TestContextBase<VertegenwoordigerPersoonsgegevensOnFVScenario, WijzigBasisgegevensRequest>
{
    protected override VertegenwoordigerPersoonsgegevensOnFVScenario InitializeScenario()
        => new();

    public WijzigNaamOnFVTestContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerPersoonsgegevensOnFVScenario scenario)
    {
        CommandResult = await new WijzigNaamOnFVRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

