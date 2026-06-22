namespace AssociationRegistry.Test.E2E.When_Voeg_Vertegenwoordiger_Toe_On_Gestopte_Vereniging;

using Admin.Api.WebApi.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.VoegVertegenwoordigerToe.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;

public class VoegVertegenwoordigerToeOnGestopteVerenigingContext : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario, VoegVertegenwoordigerToeRequest>
{

    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario InitializeScenario()
        => new();

    public VoegVertegenwoordigerToeOnGestopteVerenigingContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(VerenigingZonderEigenRechtspersoonlijkheidWerdGestoptScenario scenario)
    {
        CommandResult = await new VoegVertegenwoordigerToeOnGestopteVerenigingRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VoegVertegenwoordigerToeOnGestopteVerenigingCollection))]
public class VoegVertegenwoordigerToeOnGestopteVerenigingCollection : ICollectionFixture<VoegVertegenwoordigerToeOnGestopteVerenigingContext>
{
}
