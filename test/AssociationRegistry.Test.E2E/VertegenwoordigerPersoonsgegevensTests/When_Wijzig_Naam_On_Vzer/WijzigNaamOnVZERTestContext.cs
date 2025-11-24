namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_Vzer;

using Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevens;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;
using Scenarios.Requests.FeitelijkeVereniging.VertegenwoordigerPersoonsgegevensTests;
using Scenarios.Requests.VZER.VertegenwoordigerPersoonsgegevensTests;
using Xunit;

[CollectionDefinition(nameof(WijzigNaamOnVZERTestCollection))]
public class WijzigNaamOnVZERTestCollection : ICollectionFixture<WijzigNaamOnVZERTestContext>
{
}

public class WijzigNaamOnVZERTestContext : TestContextBase<VertegenwoordigerPersoonsgegevensOnVZERScenario, WijzigBasisgegevensRequest>
{
    protected override VertegenwoordigerPersoonsgegevensOnVZERScenario InitializeScenario()
        => new();

    public WijzigNaamOnVZERTestContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerPersoonsgegevensOnVZERScenario scenario)
    {
        CommandResult = await new WijzigNaamOnVZERRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

