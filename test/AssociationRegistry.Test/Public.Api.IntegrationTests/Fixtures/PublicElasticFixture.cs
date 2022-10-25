namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.Projections;
using Events;
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

public class PublicElasticFixture : IDisposable
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier;
    private readonly HttpClient _httpClient;
    private readonly ElasticClient _elasticClient;
    private readonly DocumentStore _documentStore;

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

        _elasticClient = ConfigureElasticClient(_testServer, VerenigingenIndexName);

        _documentStore = ConfigureDocumentStore(_testServer);
    }

    private void ConfigureBrolFeeder()
        => _testServer.Services.GetRequiredService<IVerenigingBrolFeeder>().SetStatic();

    protected void AddEvent(string vCode, VerenigingWerdGeregistreerd eventToAdd)
    {
        using var session = _documentStore.OpenSession();
        session.Events.Append(vCode, eventToAdd);
        session.SaveChanges();

        var daemon = _documentStore.BuildProjectionDaemon();
        daemon.StartAllShards().GetAwaiter().GetResult();
        daemon.WaitForNonStaleData(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();

        // Make sure all documents are properly indexed
        _elasticClient.Indices.Refresh(Indices.All);
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
        _documentStore.Dispose();

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

    private static ElasticClient ConfigureElasticClient(TestServer testServer, string verenigingenIndexName)
    {
        var client = (ElasticClient)testServer.Services.GetRequiredService(typeof(ElasticClient));

        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.Create(verenigingenIndexName);

        client.Indices.Refresh(Indices.All);
        return client;
    }

    private DocumentStore ConfigureDocumentStore(TestServer testServer)
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
        var connectionString = $@"
                    host={_configurationRoot["PostgreSQLOptions:host"]};
                    database={RootDatabase};
                    password={_configurationRoot["PostgreSQLOptions:password"]};
                    username={_configurationRoot["PostgreSQLOptions:username"]}";

        using (var connection = new NpgsqlConnection(connectionString))
        using (var cmd = connection.CreateCommand())
        {
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
    }
}
