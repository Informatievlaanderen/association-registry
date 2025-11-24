namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Wijzig_Naam_On_KBO;

using Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.MetRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;
using Scenarios.Requests.VerenigingMetRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevensTests;
using Xunit;

[CollectionDefinition(nameof(WijzigRoepnaamOnKBOTestCollection))]
public class WijzigRoepnaamOnKBOTestCollection : ICollectionFixture<WijzigRoepnaamOnKBOTestContext>
{
}

public class WijzigRoepnaamOnKBOTestContext : TestContextBase<VertegenwoordigerPersoonsgegevensOnKBOScenario, WijzigBasisgegevensRequest>
{
    protected override VertegenwoordigerPersoonsgegevensOnKBOScenario InitializeScenario()
        => new();

    public WijzigRoepnaamOnKBOTestContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerPersoonsgegevensOnKBOScenario scenario)
    {
        CommandResult = await new WijzigRoepnaamOnKBORequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

