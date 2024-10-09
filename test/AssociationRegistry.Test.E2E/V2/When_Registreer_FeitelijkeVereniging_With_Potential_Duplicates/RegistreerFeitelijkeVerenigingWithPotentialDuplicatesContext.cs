namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;

using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Scenarios.Givens;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext: TestContextBase<RegistreerFeitelijkeVerenigingRequest>
{
    private FeitelijkeVerenigingWerdGeregistreerdScenario _feitelijkeVerenigingWerdGeregistreerdScenario;
    public VCode VCode => RequestResult.VCode;

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _feitelijkeVerenigingWerdGeregistreerdScenario = new();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_feitelijkeVerenigingWerdGeregistreerdScenario);

        var requestFactory = new RegistreerFeitelijkeVerenigingWithPotentialDuplicatesRequestFactory(
            _feitelijkeVerenigingWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd);

        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }
}
