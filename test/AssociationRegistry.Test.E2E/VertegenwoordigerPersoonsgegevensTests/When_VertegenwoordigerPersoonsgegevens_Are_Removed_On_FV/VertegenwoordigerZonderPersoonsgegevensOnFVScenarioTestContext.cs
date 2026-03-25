namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.
    When_VertegenwoordigerPersoonsgegevens_Are_Removed_On_FV;

using Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevens;
using Scenarios.Requests.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevensTests;
using Xunit;

[CollectionDefinition(nameof(VertegenwoordigerZonderPersoonsgegevensOnFVTestCollection))]
public class
    VertegenwoordigerZonderPersoonsgegevensOnFVTestCollection : ICollectionFixture<
    VertegenwoordigerZonderPersoonsgegevensOnFVScenarioTestContext>
{
}

public class VertegenwoordigerZonderPersoonsgegevensOnFVScenarioTestContext : TestContextBase<
    VertegenwoordigerZonderPersoonsgegevensOnFVScenario, WijzigBasisgegevensRequest>
{
    protected override VertegenwoordigerZonderPersoonsgegevensOnFVScenario InitializeScenario()
        => new();

    public VertegenwoordigerZonderPersoonsgegevensOnFVScenarioTestContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerZonderPersoonsgegevensOnFVScenario scenario)
    {
        CommandResult =
            await new VertegenwoordigerZonderPersoonsgegevensOnFVFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
