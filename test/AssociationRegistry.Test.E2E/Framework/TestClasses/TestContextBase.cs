namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using Microsoft.Extensions.DependencyInjection;
using Public.Api.Infrastructure.ConfigurationBindings;
using Scenarios.Requests;

public abstract class TestContextBase<TRequest> : ITestContext<TRequest>
{
    public TestContextBase(FullBlownApiSetup apiSetup)
    {
        ApiSetup = apiSetup;
    }

    public abstract ValueTask InitializeAsync();
    public async ValueTask DisposeAsync()
    {
    }

    public IApiSetup ApiSetup { get; protected init; }
    public TRequest Request => RequestResult.Request;

    public async virtual ValueTask Init()
    {
    }

    public RequestResult<TRequest> RequestResult { get; set; }

    public Hosts.Configuration.ConfigurationBindings.AppSettings AdminApiAppSettings => ApiSetup.AdminApiHost.Services.GetRequiredService<Hosts.Configuration.ConfigurationBindings.AppSettings>();
    public AppSettings PublicApiAppSettings => ApiSetup.PublicApiHost.Services.GetRequiredService<AppSettings>();
}
