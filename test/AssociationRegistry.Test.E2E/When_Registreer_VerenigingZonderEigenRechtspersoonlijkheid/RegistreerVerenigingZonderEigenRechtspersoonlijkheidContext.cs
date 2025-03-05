namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Api;
using Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens;
using Scenarios.Requests.VZER;
using Vereniging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;

public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext: TestContextBase<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>
{
    private EmptyScenario _emptyScenario;
    private readonly ILogger _logger;
    public VCode VCode => RequestResult.VCode;

    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext(FullBlownApiSetup apiSetup)
    : base(apiSetup)
    {
        ApiSetup = apiSetup;
        _emptyScenario = new EmptyScenario();
    }

    public override async ValueTask InitializeAsync()
    {
        // _emptyScenario = new EmptyScenario();e
        //
        // var requestFactory = new RegistreerVZERRequestFactory();
        //
        // await ApiSetup.ExecuteGiven(_emptyScenario);
        // RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
        // await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
        // await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }

    public override async ValueTask Init()
    {
        _emptyScenario = new EmptyScenario();

        var requestFactory = new RegistreerVZERRequestFactory();
        await ApiSetup.ExecuteGiven(_emptyScenario);

        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);

        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }
}
