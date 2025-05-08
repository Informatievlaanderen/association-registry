namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid;

using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens;
using Scenarios.Requests.FeitelijkeVereniging;


public class RegistreerVerenigingMetRechtsperoonlijkheidTestContext: TestContextBase<RegistreerVerenigingUitKboRequest>
{
    public RegistreerVerenigingMetRechtsperoonlijkheidTestContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public override async Task InitializeAsync()
    {
        await ApiSetup.ExecuteGiven(new EmptyScenario());

        RequestResult = await new RegistreerVerenigingUitKboRequestFactory().ExecuteRequest(ApiSetup);

        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
