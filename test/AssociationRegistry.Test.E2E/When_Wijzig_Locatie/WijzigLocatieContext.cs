namespace AssociationRegistry.Test.E2E.When_Wijzig_Locatie;

using Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Framework.ApiSetup;
using Vereniging;
using Marten.Events;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using System;
using System.Threading.Tasks;
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

    public async ValueTask InitializeAsync()
    {
        _werdGeregistreerdScenario = new();

        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new WijzigLocatieRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public RequestResult<WijzigLocatieRequest> RequestResult { get; set; }

    public async ValueTask DisposeAsync()
    {

    }
}
