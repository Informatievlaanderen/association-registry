namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe;

using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class VoegVertegenwoordigerToeContext : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario, VoegVertegenwoordigerToeRequest>
{

    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario()
        => new();

    public VoegVertegenwoordigerToeContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new VoegVertegenwoordigerToeRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VoegVertegenwoordigerToeCollection))]
public class VoegVertegenwoordigerToeCollection : ICollectionFixture<VoegVertegenwoordigerToeContext>
{
}
