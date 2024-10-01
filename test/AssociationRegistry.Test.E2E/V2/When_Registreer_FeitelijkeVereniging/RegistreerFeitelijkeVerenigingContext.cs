namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Scenarios.Commands;
using Xunit;

public class RegistreerFeitelijkeVerenigingContext: IAsyncLifetime
{
    public FullBlownApiSetup ApiSetup { get; }
    private EmptyScenario _emptyScenario;
    public RegistreerFeitelijkeVerenigingRequest Request => _emptyScenario.Request;
    public VCode VCode => VCode.Create(_emptyScenario.VCode);

    public RegistreerFeitelijkeVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public async Task InitializeAsync()
    {
        _emptyScenario = new EmptyScenario();
        await ApiSetup.RunScenario(_emptyScenario);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public async Task DisposeAsync()
    {

    }
}
