namespace AssociationRegistry.Test.E2E.V2.When_Wijzig_Locatie;

using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Scenarios;
using Vereniging;
using E2E.Scenarios.Commands;
using Marten.Events;
using Xunit;

public class WijzigLocatieContext: IAsyncLifetime
{
    public FullBlownApiSetup ApiSetup { get; }
    private FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public WijzigLocatieRequest Request => RequestResult.Request;
    public VCode VCode => RequestResult.VCode;

    public WijzigLocatieContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public async Task InitializeAsync()
    {
        _werdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new WijzigLocatieRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public RequestResult<WijzigLocatieRequest> RequestResult { get; set; }

    public async Task DisposeAsync()
    {

    }
}
