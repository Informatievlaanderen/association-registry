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
    public RegistreerVerenigingMetRechtsperoonlijkheidTestContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public override async ValueTask Init()
    {
        await ApiSetup.ExecuteGiven(new EmptyScenario());

        RequestResult = await new RegistreerVerenigingUitKboRequestFactory().ExecuteRequest(ApiSetup);

        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }

    public override async ValueTask InitializeAsync()
    {
    }
}
