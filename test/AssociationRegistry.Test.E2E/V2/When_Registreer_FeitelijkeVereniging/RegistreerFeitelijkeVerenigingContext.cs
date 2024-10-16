namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging;

using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Framework.ApiSetup;
using Vereniging;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens;
using Scenarios.Requests.FeitelijkeVereniging;

public class RegistreerFeitelijkeVerenigingTestContext: TestContextBase<RegistreerFeitelijkeVerenigingRequest>
{
    private EmptyScenario _emptyScenario;
    public VCode VCode => RequestResult.VCode;

    public RegistreerFeitelijkeVerenigingTestContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _emptyScenario = new EmptyScenario();
    }

    public override async Task InitializeAsync()
    {
        _emptyScenario = new EmptyScenario();

        var requestFactory = new RegistreerFeitelijkeVerenigingRequestFactory();

        await ApiSetup.ExecuteGiven(_emptyScenario);
        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
