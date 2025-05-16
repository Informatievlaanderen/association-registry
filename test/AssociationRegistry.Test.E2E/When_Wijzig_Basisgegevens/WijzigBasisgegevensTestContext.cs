namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens;

using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Events;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;

public class WijzigBasisgegevensTestContext: TestContextBase<WijzigBasisgegevensRequest>
{
    private FeitelijkeVerenigingWerdGeregistreerdScenario _werdGeregistreerdScenario;
    public FeitelijkeVerenigingWerdGeregistreerd RegistratieData => _werdGeregistreerdScenario.FeitelijkeVerenigingWerdGeregistreerd;
    public VCode VCode => RequestResult.VCode;

    public WijzigBasisgegevensTestContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public override async Task InitializeAsync()
    {
        _werdGeregistreerdScenario = new(true);

        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new WijzigBasisgegevensRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
