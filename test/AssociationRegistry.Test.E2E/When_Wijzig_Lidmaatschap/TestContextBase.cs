namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap;

using Framework.ApiSetup;
using Framework.TestClasses;
using JasperFx.Core;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Public.Api.Infrastructure.ConfigurationBindings;
using Scenarios.Requests;
using Vereniging;
using Xunit;

public abstract class TestContextBase<TScenario, TCommandRequest> : IDisposable, IAsyncLifetime
where TScenario : IScenario
{
    protected TestContextBase(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public FullBlownApiSetup ApiSetup { get; }
    public VCode VCode => CommandResult.VCode;
    protected abstract TScenario InitializeScenario();
    public CommandResult<TCommandRequest> CommandResult { get; protected set; }
    public TCommandRequest CommandRequest => CommandResult.Request;
    private IVCodeService VCodeService => ApiSetup.VCodeService;
    public TScenario Scenario { get; private set; }

    public AppSettings PublicApiAppSettings =>
        ApiSetup.PublicApiHost.Services.GetRequiredService<AppSettings>();
    public Hosts.Configuration.ConfigurationBindings.AppSettings AdminApiAppSettings =>
        ApiSetup.AdminApiHost.Services.GetRequiredService<Hosts.Configuration.ConfigurationBindings.AppSettings>();

    public async ValueTask InitializeAsync()
    {
        Scenario = InitializeScenario();
        var executedEvents = await ApiSetup.ExecuteGiven(Scenario);

        if (executedEvents.Length != 0)
        {
            await ApiSetup.AdminProjectionDaemon.WaitForNonStaleData(10.Seconds());
            await ApiSetup.AcmProjectionDaemon.WaitForNonStaleData(10.Seconds());
            await ApiSetup.PublicProjectionDaemon.WaitForNonStaleData(10.Seconds());
        }

        await ExecuteScenario(Scenario);

        await ApiSetup.AdminProjectionDaemon.WaitForNonStaleData(10.Seconds());
        await ApiSetup.AcmProjectionDaemon.WaitForNonStaleData(10.Seconds());
        await ApiSetup.PublicProjectionDaemon.WaitForNonStaleData(10.Seconds());

        await ApiSetup.AdminApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
        await ApiSetup.PublicApiHost.Services.GetRequiredService<IElasticClient>().Indices.RefreshAsync(Indices.All);
    }


    protected abstract ValueTask ExecuteScenario(TScenario scenario);

    public void Dispose()
    {
    }

    public async ValueTask DisposeAsync()
    {
    }
}
