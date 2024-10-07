namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;

using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using E2E.Scenarios;
using E2E.Scenarios.Commands;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Vereniging;

public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext: TestContextBase<RegistreerFeitelijkeVerenigingRequest>
{
    private FeitelijkeVerenigingWerdGeregistreerdScenario _verenigingWerdGeregistreerdScenario;
    public VCode VCode => RequestResult.VCode;

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _verenigingWerdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_verenigingWerdGeregistreerdScenario);

        var requestFactory = new RegistreerFeitelijkeVerenigingWithPotentialDuplicatesRequestFactory(
            _verenigingWerdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd);

        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    }
}
