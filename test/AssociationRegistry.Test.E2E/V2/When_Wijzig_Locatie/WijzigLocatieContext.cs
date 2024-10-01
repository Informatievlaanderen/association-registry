namespace AssociationRegistry.Test.E2E.V2.When_Wijzig_Locatie;

using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Scenarios.Commands;
using Xunit;

public class WijzigLocatieContext: IAsyncLifetime
{
    public FullBlownApiSetup ApiSetup { get; }
    private FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public WijzigLocatieRequest Request => _werdGeregistreerdScenario.Request;
    public VCode VCode => VCode.Create(_werdGeregistreerdScenario.VCode);

    public WijzigLocatieContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public async Task InitializeAsync()
    {
        _werdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario(new RegistreerFeitelijkeVerenigingCommandFactory());
        await ApiSetup.RunScenario(_werdGeregistreerdScenario);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public async Task DisposeAsync()
    {

    }
}
