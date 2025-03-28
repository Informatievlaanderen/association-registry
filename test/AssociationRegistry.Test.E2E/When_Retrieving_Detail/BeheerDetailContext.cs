namespace AssociationRegistry.Test.E2E.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens;
using AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;
using AssociationRegistry.Vereniging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;

public class BeheerDetailContext: TestContextBase<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>
{
    private EmptyScenario _emptyScenario;
    private readonly ILogger _logger;
    public VCode VCode => RequestResult.VCode;

    public BeheerDetailContext(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
        _emptyScenario = new EmptyScenario();

        apiSetup.RegisterContext(this);
    }

    public override async Task InitializeAsync()
    { }

    public override async Task Init()
    {
        var logger = ApiSetup.AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

        _emptyScenario = new EmptyScenario();

        var requestFactory = new RegistreerVZERRequestFactory();

        var daemon = ApiSetup.AdminProjectionDaemon;

        logger.LogInformation("Starting up shards (running: {IsRunning})", daemon.IsRunning);


        await ApiSetup.ExecuteGiven(_emptyScenario);

        logger.LogInformation("Executing registreer feitelijke vereniging request");

        RequestResult = await requestFactory.ExecuteRequest(ApiSetup);


        await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(10));

        var agents = daemon.CurrentAgents().Select(x => new
        {
            x.Name.Identity, x.Position, x.Status
        });

        logger.LogInformation("Daemon agents {@Says}", agents.Select(x => $" {x.Identity}: {x.Position} ({x.Status})|"));

        logger.LogInformation("Waiting for non stale data...(running: {IsRunning})", daemon.IsRunning);

        logger.LogInformation("Refreshing indices");

        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);

        logger.LogInformation("Initialisation complete");
    }
}
