namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using Microsoft.Extensions.DependencyInjection;
using Public.Api.Infrastructure.ConfigurationBindings;
using Scenarios.Commands;

public abstract class TestContextBase<TRequest> : ITestContext<TRequest>
{
    public abstract Task InitializeAsync();
    public async Task DisposeAsync()
    {
    }

    public IApiSetup ApiSetup { get; protected init; }
    public TRequest Request => RequestResult.Request;
    public RequestResult<TRequest> RequestResult { get; protected set; }

    public AppSettings PublicApiAppSettings => ApiSetup.PublicApiHost.Services.GetRequiredService<AppSettings>();
}
