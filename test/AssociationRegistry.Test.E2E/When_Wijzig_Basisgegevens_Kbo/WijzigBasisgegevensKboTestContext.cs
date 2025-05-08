namespace AssociationRegistry.Test.E2E.When_Wijzig_Basisgegevens_Kbo;

using Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using Events;
using Framework.ApiSetup;
using Framework.TestClasses;
using Vereniging;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.MetRechtspersoonlijkheid;
using WijzigBasisgegevensRequestFactory = Scenarios.Requests.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevensRequestFactory;

public class WijzigBasisgegevensKboTestContext : TestContextBase<WijzigBasisgegevensRequest>
{
    public const string Name = "WijzigBasisgegevensKboTestContext";
    private VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _werdGeregistreerdScenario;

    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd RegistratieData
        => _werdGeregistreerdScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

    public VCode VCode => RequestResult.VCode;

    public WijzigBasisgegevensKboTestContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public override async ValueTask InitializeAsync()
    {
        _werdGeregistreerdScenario = new();
        await ApiSetup.ExecuteGiven(_werdGeregistreerdScenario);

        RequestResult = await new WijzigBasisgegevensRequestFactory(_werdGeregistreerdScenario).ExecuteRequest(ApiSetup);
        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
