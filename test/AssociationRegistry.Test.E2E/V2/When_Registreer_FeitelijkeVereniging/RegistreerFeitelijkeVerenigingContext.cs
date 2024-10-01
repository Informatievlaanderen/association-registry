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
    public RegistreerFeitelijkeVerenigingRequest Request => RequestResult.Request;
    public VCode VCode => RequestResult.VCode;

    public RegistreerFeitelijkeVerenigingContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _emptyScenario = new EmptyScenario();
    }

    public async Task InitializeAsync()
    {
        _emptyScenario = new EmptyScenario();

        var requestFactory = new RegistreerFeitelijkeVerenigingRequestFactory();

        await ApiSetup.ExecuteGiven(_emptyScenario);
        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public RequestResult<RegistreerFeitelijkeVerenigingRequest> RequestResult { get; set; }

    public async Task DisposeAsync()
    {

    }
}
