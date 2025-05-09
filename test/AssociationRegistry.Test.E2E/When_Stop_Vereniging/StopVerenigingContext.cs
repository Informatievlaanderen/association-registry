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
    public const string Name = "StopVerenigingContext";
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public VCode VCode => CommandResult.VCode;

    public StopVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _werdGeregistreerdScenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        CommandResult = await new StopVerenigingRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.RefreshIndices();
    }
}
