namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.
    When_VertegenwoordigerPersoonsgegevens_Are_Removed;

using Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;
using Scenarios.Requests.VZER.VertegenwoordigerPersoonsgegevensTests;
using Xunit;

[CollectionDefinition(nameof(VertegenwoordigerZonderPersoonsgegevensTestCollection))]
public class
    VertegenwoordigerZonderPersoonsgegevensTestCollection : ICollectionFixture<
    VertegenwoordigerZonderPersoonsgegevensOnVZERScenarioTestContext>
{
}

public class VertegenwoordigerZonderPersoonsgegevensOnVZERScenarioTestContext : TestContextBase<
    VertegenwoordigerZonderPersoonsgegevensOnVZERScenario, WijzigBasisgegevensRequest>
{
    protected override VertegenwoordigerZonderPersoonsgegevensOnVZERScenario InitializeScenario()
        => new();

    public VertegenwoordigerZonderPersoonsgegevensOnVZERScenarioTestContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerZonderPersoonsgegevensOnVZERScenario scenario)
    {
        CommandResult =
            await new VertegenwoordigerZonderPersoonsgegevensOnVZERFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
