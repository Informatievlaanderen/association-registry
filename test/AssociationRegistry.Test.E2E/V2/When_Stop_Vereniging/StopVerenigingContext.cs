namespace AssociationRegistry.Test.E2E.V2.When_Stop_Vereniging;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Framework.ApiSetup;
using Vereniging;
using Framework.TestClasses;
using Marten.Events;
using Scenarios.Givens;
using Scenarios.Requests;

public class StopVerenigingContext: TestContextBase<StopVerenigingRequest>
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public VCode VCode => RequestResult.VCode;

    public StopVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _werdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new StopVerenigingRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }
}
