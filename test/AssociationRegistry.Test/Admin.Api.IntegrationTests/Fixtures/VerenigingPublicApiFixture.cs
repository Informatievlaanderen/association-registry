namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Admin.Api;
using Framework.Helpers;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

public class VerenigingAdminApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private TestServer? _testServer;

    public HttpClient? HttpClient { get; private set; }
    public IDocumentStore? DocumentStore { get; private set; }

    public async Task InitializeAsync()
    {
        var configuration = GetConfiguration();

        await WaitFor.PostGreSQLToBecomeAvailable(
            LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitForPostgresTestLogger"),
            GetRootConnectionString(configuration)
        );

        var hostBuilder = new WebHostBuilder();

        hostBuilder.UseConfiguration(configuration);
        hostBuilder.UseStartup<Startup>();
        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
        hostBuilder.UseTestServer();

        _testServer = new TestServer(hostBuilder);

        HttpClient = _testServer.CreateClient();
        DocumentStore = ((IDocumentStore?)_testServer.Services.GetService(typeof(IDocumentStore)))!;
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

    private static string GetRootConnectionString(IConfiguration configurationRoot)
        => $"host={configurationRoot["PostgreSQLOptions:host"]};" +
           $"database={RootDatabase};" +
           $"password={configurationRoot["PostgreSQLOptions:password"]};" +
           $"username={configurationRoot["PostgreSQLOptions:username"]}";

    public Task DisposeAsync()
        => Task.CompletedTask;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        HttpClient?.Dispose();
        _testServer?.Dispose();
    }
}
