namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.Projections;
using Events;
using Helpers;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Npgsql;
using Xunit;
using Xunit.Sdk;

public class PublicElasticFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier;
    private readonly HttpClient _httpClient;
    private readonly IElasticClient _elasticClient;
    private DocumentStore? _documentStore;

    private readonly TestServer _testServer;
    private readonly IConfigurationRoot _configurationRoot;

    private string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"];

    protected PublicElasticFixture(string identifier)
    {
        _identifier += "_" + identifier.ToLowerInvariant();
        GoToRootDirectory();

        _configurationRoot = SetConfigurationRoot();

        _testServer = ConfigureTestServer();

        ConfigureBrolFeeder();

        _httpClient = _testServer.CreateClient();

        _elasticClient = CreateElasticClient(_testServer);
    }

    private void ConfigureBrolFeeder()
        => _testServer.Services.GetRequiredService<IVerenigingBrolFeeder>().SetStatic();

    protected async Task AddEvent(string vCode, VerenigingWerdGeregistreerd eventToAdd)
    {
        if (_documentStore is not { })
            throw new NullException("DocumentStore cannot be null when adding an event");

        await using var session = _documentStore.OpenSession();
        session.Events.Append(vCode, eventToAdd);
        await session.SaveChangesAsync();

        var daemon = await _documentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();
        await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(10));

        // Make sure all documents are properly indexed
        await _elasticClient.Indices.RefreshAsync(Indices.All);
    }

    public async Task<string> Search(string uri)
    {
        var responseMessage = await GetResponseMessage(uri);
        return await responseMessage.Content.ReadAsStringAsync();
    }

    public async Task<HttpResponseMessage> GetResponseMessage(string uri)
    {
        if (_httpClient is null)
            throw new NullReferenceException("HttpClient needs to be set before performing a get");

        return await _httpClient.GetAsync(uri);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _httpClient.Dispose();
        _testServer.Dispose();
        _documentStore?.Dispose();

        DropDatabase();

        _elasticClient.Indices.Delete(VerenigingenIndexName);
        _elasticClient.Indices.Refresh(Indices.All);
    }

    private static void GoToRootDirectory()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");
        Directory.SetCurrentDirectory(rootDirectory);
    }

    private IConfigurationRoot SetConfigurationRoot()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);
        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] += _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] += _identifier;
        return tempConfiguration;
    }

    private TestServer ConfigureTestServer()
    {
        IWebHostBuilder hostBuilder = new WebHostBuilder();
        hostBuilder.UseConfiguration(_configurationRoot);
        hostBuilder.UseStartup<Startup>();
        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
        hostBuilder.UseTestServer();
        return new TestServer(hostBuilder);
    }

    private static IElasticClient CreateElasticClient(TestServer testServer)
        => (IElasticClient)testServer.Services.GetRequiredService(typeof(ElasticClient));

    private static void ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
    {
        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.Create(verenigingenIndexName);

        client.Indices.Refresh(Indices.All);
    }

    private DocumentStore ConfigureDocumentStore()
    {
        return DocumentStore.For(
            opts =>
            {
                var connectionString = $@"
                    host={_configurationRoot["PostgreSQLOptions:host"]};
                    database={_configurationRoot["PostgreSQLOptions:database"]};
                    password={_configurationRoot["PostgreSQLOptions:password"]};
                    username={_configurationRoot["PostgreSQLOptions:username"]}";

                var rootConnectionString = $@"
                    host={_configurationRoot["PostgreSQLOptions:host"]};
                    database={RootDatabase};
                    password={_configurationRoot["PostgreSQLOptions:password"]};
                    username={_configurationRoot["PostgreSQLOptions:username"]}";

                opts.Connection(connectionString);

                opts.CreateDatabasesForTenants(c =>
                {
                    c.MaintenanceDatabase(rootConnectionString);
                    c.ForTenant()
                        .CheckAgainstPgDatabase()
                        .WithOwner(_configurationRoot["PostgreSQLOptions:username"]);
                });

                opts.Events.StreamIdentity = StreamIdentity.AsString;

                opts.Projections.Add(
                    new MartenSubscription(
                        new MartenEventsConsumer(_testServer.Services)), ProjectionLifecycle.Async);
            });
    }

    private void DropDatabase()
    {
        using var connection = new NpgsqlConnection(GetConnectionString());
        using var cmd = connection.CreateCommand();
        try
        {
            connection.Open();
            // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
            cmd.CommandText += $"DROP DATABASE IF EXISTS {_configurationRoot["PostgreSQLOptions:database"]} WITH (FORCE);";
            cmd.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    private string GetConnectionString()
    {
        var connectionString = $@"
                    host={_configurationRoot["PostgreSQLOptions:host"]};
                    database={RootDatabase};
                    password={_configurationRoot["PostgreSQLOptions:password"]};
                    username={_configurationRoot["PostgreSQLOptions:username"]}";
        return connectionString;
    }

    public virtual async Task InitializeAsync()
    {
        await WaitFor.ElasticSearchToBecomeAvailable(_elasticClient, LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitForElasticSearchTestLogger"));
        ConfigureElasticClient(_elasticClient, VerenigingenIndexName);

        await WaitFor.PostGreSQLToBecomeAvailable(
            LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitForPostgresTestLogger"),
            GetConnectionString()
        );
        _documentStore = ConfigureDocumentStore();
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
