namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using Microsoft.Extensions.DependencyInjection;
using Public.Api.Infrastructure.ConfigurationBindings;
using Scenarios.Requests;

public abstract class TestContextBase<TRequest> : ITestContext<TRequest>
{
    public abstract Task InitializeAsync();
    public async Task DisposeAsync()
    {
    }

    public IApiSetup ApiSetup { get; protected init; }
    public TRequest Request => RequestResult.Request;
    public RequestResult<TRequest> RequestResult { get; protected set; }

    public Hosts.Configuration.ConfigurationBindings.AppSettings AdminApiAppSettings => ApiSetup.AdminApiHost.Services.GetRequiredService<Hosts.Configuration.ConfigurationBindings.AppSettings>();
    public AppSettings PublicApiAppSettings => ApiSetup.PublicApiHost.Services.GetRequiredService<AppSettings>();
}
