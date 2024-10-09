namespace AssociationRegistry.Test.E2E.V2.When_Wijzig_Basisgegevens_Kbo;

using Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Events;
using Framework.ApiSetup;
using Framework.TestClasses;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens;
using Scenarios.Requests;
using Vereniging;

public class WijzigBasisgegevensKboTestContext : TestContextBase<WijzigBasisgegevensRequest>
{
    private VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _werdGeregistreerdScenario;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd RegistratieData
        => _werdGeregistreerdScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public VCode VCode => RequestResult.VCode;

    public WijzigBasisgegevensKboTestContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public override async Task InitializeAsync()
    {
        _werdGeregistreerdScenario = new();

        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);
        RequestResult = await new WijzigBasisgegevensKboRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
