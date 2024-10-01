namespace AssociationRegistry.Test.E2E.V2.When_Stop_Vereniging;

using Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Xunit;

public class StopVerenigingContext: IAsyncLifetime
{
    public FullBlownApiSetup ApiSetup { get; }
    private FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public StopVerenigingRequest Request => _werdGeregistreerdScenario.Request;
    public VCode VCode => VCode.Create(_werdGeregistreerdScenario.VCode);

    public StopVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public async Task InitializeAsync()
    {
        _werdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        await ApiSetup.RunScenario(_werdGeregistreerdScenario);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public async Task DisposeAsync()
    {

    }
}
