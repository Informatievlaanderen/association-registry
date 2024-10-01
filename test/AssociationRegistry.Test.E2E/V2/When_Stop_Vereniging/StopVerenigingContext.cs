namespace AssociationRegistry.Test.E2E.V2.When_Stop_Vereniging;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios;
using Vereniging;
using E2E.Scenarios.Commands;
using Marten.Events;
using Xunit;

public class StopVerenigingContext: IAsyncLifetime
{
    public FullBlownApiSetup ApiSetup { get; }
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public StopVerenigingRequest Request => RequestResult.Request;
    public VCode VCode => RequestResult.VCode;

    public StopVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _werdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
    }

    public async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));

        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new StopVerenigingRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public RequestResult<StopVerenigingRequest> RequestResult { get; set; }

    public async Task DisposeAsync()
    {

    }
}
