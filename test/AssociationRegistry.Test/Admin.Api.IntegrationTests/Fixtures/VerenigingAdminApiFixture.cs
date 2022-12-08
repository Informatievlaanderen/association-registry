namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Admin.Api;
using Framework.Helpers;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

public class VerenigingAdminApiFixture : IDisposable
{
    public HttpClient? HttpClient { get; }
    private readonly TestServer? _testServer;
    public IDocumentStore DocumentStore { get; }

    public VerenigingAdminApiFixture()
    {
        var configuration = GetConfiguration();
        var hostBuilder = new WebHostBuilder();

        hostBuilder.UseConfiguration(configuration);
        hostBuilder.UseStartup<Startup>();
        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
        hostBuilder.UseTestServer();

        _testServer = new TestServer(hostBuilder);

        HttpClient = _testServer.CreateClient();
        DocumentStore = _testServer.Services.GetRequiredService<IDocumentStore>();
    }

    private static IConfigurationRoot GetConfiguration()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        var builder = new ConfigurationBuilder()
            .SetBasePath(rootDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var configurationRoot = builder.Build();
        return configurationRoot;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        HttpClient?.Dispose();
        _testServer?.Dispose();
    }
}
