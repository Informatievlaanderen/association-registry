namespace AssociationRegistry.Test.E2E.V2.When_Registreer_VerenigingMetRechtspersoonlijkheid;

using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.V2.Scenarios.Givens;
using AssociationRegistry.Test.E2E.V2.Scenarios.Requests.FeitelijkeVereniging;
using AssociationRegistry.Vereniging;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;

public class RegistreerVerenigingMetRechtspersoonlijkheidTestContext: TestContextBase<RegistreerVerenigingUitKboRequest>
{
    private EmptyScenario _emptyScenario;
    public VCode VCode => RequestResult.VCode;

    public RegistreerVerenigingMetRechtspersoonlijkheidTestContext(FullBlownApiSetup apiSetup)
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
