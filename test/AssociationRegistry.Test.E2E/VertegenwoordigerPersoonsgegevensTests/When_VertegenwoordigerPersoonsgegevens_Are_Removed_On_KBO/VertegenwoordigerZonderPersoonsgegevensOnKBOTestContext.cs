namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_VertegenwoordigerPersoonsgegevens_Are_Removed_On_KBO;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.MetRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevens;
using AssociationRegistry.Test.E2E.Scenarios.Requests.VerenigingMetRechtspersoonlijkheid.VertegenwoordigerPersoonsgegevensTests;
using Xunit;

[CollectionDefinition(nameof(VertegenwoordigerZonderPersoonsgegevensOnKBOTestCollection))]
public class VertegenwoordigerZonderPersoonsgegevensOnKBOTestCollection : ICollectionFixture<VertegenwoordigerZonderPersoonsgegevensOnKBOTestContext>
{
}

public class VertegenwoordigerZonderPersoonsgegevensOnKBOTestContext : TestContextBase<VertegenwoordigerZonderPersoonsgegevensOnKBOScenario, WijzigBasisgegevensRequest>
{
    protected override VertegenwoordigerZonderPersoonsgegevensOnKBOScenario InitializeScenario()
        => new();

    public VertegenwoordigerZonderPersoonsgegevensOnKBOTestContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VertegenwoordigerZonderPersoonsgegevensOnKBOScenario scenario)
    {
        CommandResult = await new WijzigRoepnaamVertegenwoordigerZonderPersoonsgegevensOnKBORequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

