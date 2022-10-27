namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Admin.Api;
using Helpers;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

public class VerenigingAdminApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    public HttpClient? HttpClient { get; private set; }
    public IDocumentStore? DocumentStore { get; private set; }
    private TestServer? _testServer;
    private readonly IConfigurationRoot _configurationRoot;
    private readonly IWebHostBuilder _hostBuilder;

    public VerenigingAdminApiFixture()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        Directory.SetCurrentDirectory(rootDirectory);

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        _configurationRoot = builder.Build();

        _hostBuilder = new WebHostBuilder();

        _hostBuilder.UseConfiguration(_configurationRoot);
        _hostBuilder.UseStartup<Startup>();

        _hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());

        _hostBuilder.UseTestServer();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        HttpClient?.Dispose();
        _testServer?.Dispose();
    }

    public async Task InitializeAsync()
    {
        await WaitFor.PostGreSQLToBecomeAvailable(
            LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitForPostgresTestLogger"),
            GetRootConnectionString()
        );

        _testServer = new TestServer(_hostBuilder);

        HttpClient = _testServer.CreateClient();
        DocumentStore = ((IDocumentStore?)_testServer.Services.GetService(typeof(IDocumentStore)))!;
    }

    private string GetRootConnectionString()
    {
        var rootConnectionString = $@"
                     host={_configurationRoot["PostgreSQLOptions:host"]};
                     database={RootDatabase};
                     password={_configurationRoot["PostgreSQLOptions:password"]};
                     username={_configurationRoot["PostgreSQLOptions:username"]}";
        return rootConnectionString;
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
