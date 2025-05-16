namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap;

using Framework.ApiSetup;
using Microsoft.Extensions.DependencyInjection;
using Public.Api.Infrastructure.ConfigurationBindings;
using Scenarios.Requests;
using Vereniging;
using Xunit;

public abstract class TestContextBase<TScenario, TCommandRequest> : IDisposable, IAsyncLifetime
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
        await ExecuteScenario(Scenario);
    }

    protected abstract ValueTask ExecuteScenario(TScenario scenario);

    public void Dispose()
    {
    }

    public async ValueTask DisposeAsync()
    {
    }
}
