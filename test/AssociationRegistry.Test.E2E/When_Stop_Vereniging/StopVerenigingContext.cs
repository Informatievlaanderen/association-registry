namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging;
using Marten.Events;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;

public class StopVerenigingContext: TestContextBase<StopVerenigingRequest>
{
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public VCode VCode => RequestResult.VCode;

    public StopVerenigingContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
        ApiSetup = apiSetup;
        _werdGeregistreerdScenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
    }

    public override async ValueTask Init()    {
        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new StopVerenigingRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }
}
