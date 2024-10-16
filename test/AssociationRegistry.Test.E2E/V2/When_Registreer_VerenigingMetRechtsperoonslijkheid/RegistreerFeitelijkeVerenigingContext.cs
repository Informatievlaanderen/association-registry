namespace AssociationRegistry.Test.E2E.V2.When_Registreer_VerenigingMetRechtsperoonslijkheid;

using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using Events;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

public class RegistreerVerenigingMetRechtsperoonlijkheidTestContext: TestContextBase<RegistreerVerenigingUitKboRequest>
{
    public VCode VCode => _werdGeregistreerdScenario.VCode;
    private FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public FeitelijkeVerenigingWerdGeregistreerd RegistratieData => _werdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd;

    public RegistreerVerenigingMetRechtsperoonlijkheidTestContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public override async Task InitializeAsync()
    {
        _werdGeregistreerdScenario = new(true);
        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);

        RequestKboResult = await new RegistreerVerenigingUitKboRequestFactory().ExecuteRequest(ApiSetup);

        await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
