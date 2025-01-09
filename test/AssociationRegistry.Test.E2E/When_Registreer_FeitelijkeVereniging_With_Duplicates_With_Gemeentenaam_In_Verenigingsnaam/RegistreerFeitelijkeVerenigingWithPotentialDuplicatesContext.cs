namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Duplicates_With_Gemeentenaam_In_Verenigingsnaam;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
using AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;

public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext: TestContextBase<RegistreerFeitelijkeVerenigingRequest>
{
    private MultipleWerdenGeregistreerdWithGemeentenaamInVerenigingsnaamScenario _scenario;
    public VCode VCode => RequestResult.VCode;

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _scenario = new();
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(_scenario);

        var requestFactory = new RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamFactory();

        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminProjectionHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.AllIndices);
    }
}
