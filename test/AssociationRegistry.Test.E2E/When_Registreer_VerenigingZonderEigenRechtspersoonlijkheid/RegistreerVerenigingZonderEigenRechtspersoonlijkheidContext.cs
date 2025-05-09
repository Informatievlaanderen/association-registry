namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;

using Admin.Api;
using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Scenarios.Givens;
using Scenarios.Requests.VZER;
using Vereniging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using When_Wijzig_Lidmaatschap;
using Xunit;

[CollectionDefinition(nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection))]
public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection : ICollectionFixture<RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext>
{
}
public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext : TestContextBase<EmptyScenario, RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>
{

    protected override EmptyScenario InitializeScenario()
        => new();

    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(EmptyScenario scenario)
    {
        CommandResult = await new RegistreerVZERRequestFactory().ExecuteRequest(ApiSetup);
    }


    /// <summary>
    ///
    /// </summary>


    // public const string Name = "RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext";
    // private EmptyScenario _emptyScenario;
    // private readonly ILogger _logger;
    // public VCode VCode => CommandResult.VCode;
    //
    // public RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext(FullBlownApiSetup apiSetup)
    // {
    //     ApiSetup = apiSetup;
    //     _emptyScenario = new EmptyScenario();
    //
    //     apiSetup.RegisterContext(this);
    // }
    //
    // public override async ValueTask InitializeAsync()
    // {
    //     // _emptyScenario = new EmptyScenario();e
    //     //
    //     // var requestFactory = new RegistreerVZERRequestFactory();
    //     //
    //     // await ApiSetup.ExecuteGiven(_emptyScenario);
    //     // RequestResult = await requestFactory.ExecuteRequest(ApiSetup);
    //     // await ApiSetup.AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(10));
    //     // await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    // }
    //
    // public override async Task Init()
    // {
    //     var logger = ApiSetup.AdminApiHost.Services.GetRequiredService<ILogger<Program>>();
    //
    //     _emptyScenario = new EmptyScenario();
    //
    //     var requestFactory = new RegistreerVZERRequestFactory();
    //
    //     var daemon = ApiSetup.AdminProjectionDaemon;
    //
    //     logger.LogInformation("Starting up shards (running: {IsRunning})", daemon.IsRunning);
    //
    //
    //     await ApiSetup.ExecuteGiven(_emptyScenario);
    //
    //     logger.LogInformation("Executing registreer feitelijke vereniging request");
    //
    //     CommandResult = await requestFactory.ExecuteRequest(ApiSetup);
    //
    //
    //     await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(10));
    //
    //     var agents = daemon.CurrentAgents().Select(x => new
    //     {
    //         x.Name.Identity, x.Position, x.Status
    //     });
    //
    //     logger.LogInformation("Daemon agents {@Says}", agents.Select(x => $" {x.Identity}: {x.Position} ({x.Status})|"));
    //
    //     logger.LogInformation("Waiting for non stale data...(running: {IsRunning})", daemon.IsRunning);
    //
    //     logger.LogInformation("Refreshing indices");
    //
    //     await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    //
    //     logger.LogInformation("Initialisation complete");
    // }
}
