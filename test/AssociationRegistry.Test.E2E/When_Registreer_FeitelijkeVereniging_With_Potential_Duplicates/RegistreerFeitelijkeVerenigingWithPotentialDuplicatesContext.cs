namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;

using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging;
using Marten.Events;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;

public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext: TestContextBase<RegistreerFeitelijkeVerenigingRequest>
{
    private FeitelijkeVerenigingWerdGeregistreerdScenario _feitelijkeVerenigingWerdGeregistreerdScenario;
    public VCode VCode => RequestResult.VCode;

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _feitelijkeVerenigingWerdGeregistreerdScenario = new();
    }

    public override async ValueTask InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_feitelijkeVerenigingWerdGeregistreerdScenario);

        var requestFactory = new RegistreerFeitelijkeVerenigingWithPotentialDuplicatesRequestFactory(
            _feitelijkeVerenigingWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd);

        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }
}
