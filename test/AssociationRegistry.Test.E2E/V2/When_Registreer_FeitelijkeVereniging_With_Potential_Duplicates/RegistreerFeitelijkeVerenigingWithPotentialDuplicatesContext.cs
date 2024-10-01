namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;

using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Scenarios;
using Scenarios.Commands;
using V2.When_Stop_Vereniging;
using Vereniging;
using Xunit;

public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext: ITestContext<RegistreerFeitelijkeVerenigingRequest>
{
    public IApiSetup ApiSetup { get; }
    private FeitelijkeVerenigingWerdGeregistreerdScenario _verenigingWerdGeregistreerdScenario;
    public RegistreerFeitelijkeVerenigingRequest Request => RequestResult.Request;
    public VCode VCode => RequestResult.VCode;

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _verenigingWerdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
    }

    public async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_verenigingWerdGeregistreerdScenario);

        var requestFactory = new RegistreerFeitelijkeVerenigingWithPotentialDuplicatesRequestFactory(
            _verenigingWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd);

        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }

    public RequestResult<RegistreerFeitelijkeVerenigingRequest> RequestResult { get; set; }

    public async Task DisposeAsync()
    {

    }
}
