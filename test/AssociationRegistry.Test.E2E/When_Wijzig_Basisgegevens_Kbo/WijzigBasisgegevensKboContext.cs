namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo;

using Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Events;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.MetRechtspersoonlijkheid;
using Xunit;
using WijzigBasisgegevensRequestFactory = Scenarios.Requests.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevensRequestFactory;

[CollectionDefinition(nameof(WijzigBasisgegevensKbocollection))]
public class WijzigBasisgegevensKbocollection : ICollectionFixture<WijzigBasisgegevensKboContext>
{
}


public class WijzigBasisgegevensKboContext : TestContextBase<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario, WijzigBasisgegevensRequest>
{

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd RegistratieData
        => Scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    protected override VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario()
        => new();


    public WijzigBasisgegevensKboContext(FullBlownApiSetup apiSetup) :  base(apiSetup)
    {
    }


    protected override async ValueTask ExecuteScenario(VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new WijzigBasisgegevensRequestFactory(Scenario).ExecuteRequest(ApiSetup);
    }
}
