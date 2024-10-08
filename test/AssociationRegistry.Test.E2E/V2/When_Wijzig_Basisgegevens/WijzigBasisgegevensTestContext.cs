namespace AssociationRegistry.Test.E2E.V2.When_Wijzig_Basisgegevens;

using Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using Events;
using Framework.ApiSetup;
using Framework.TestClasses;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens;
using Scenarios.Requests;
using Vereniging;

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
        _werdGeregistreerdScenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new WijzigBasisgegevensRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
