namespace AssociationRegistry.Test.E2E.When_Verwijder_Vereniging;

using Admin.Api.Verenigingen.Verwijder.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Vereniging;

public class VerwijderVerenigingContext: TestContextBase<VerwijderVerenigingRequest>
{
    public const string Name = "VerwijderVerenigingContext";
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public VCode VCode => RequestResult.VCode;

    public VerwijderVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _werdGeregistreerdScenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new VerwijderVerenigingRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.RefreshIndices();
    }
}
