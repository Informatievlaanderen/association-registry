namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using Microsoft.Extensions.DependencyInjection;
using Public.Api.Infrastructure.ConfigurationBindings;
using Scenarios.Requests;
using Xunit;

public class TestContext<TRequest> : ITestContext2<TRequest>
{
    public async ValueTask DisposeAsync()
    {
    }

    public IApiSetup ApiSetup { get; protected init; }
    public TRequest Request => CommandResult.Request;

    public CommandResult<TRequest> CommandResult { get; set; }

    public Hosts.Configuration.ConfigurationBindings.AppSettings AdminApiAppSettings => ApiSetup.AdminApiHost.Services.GetRequiredService<Hosts.Configuration.ConfigurationBindings.AppSettings>();
    public AppSettings PublicApiAppSettings => ApiSetup.PublicApiHost.Services.GetRequiredService<AppSettings>();
}

public interface ITestContext2<TRequest>
{
    IApiSetup ApiSetup { get; }
    TRequest Request { get; }
    CommandResult<TRequest> CommandResult { get; set; }
}

