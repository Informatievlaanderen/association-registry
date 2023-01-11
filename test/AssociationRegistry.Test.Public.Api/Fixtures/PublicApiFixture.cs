namespace AssociationRegistry.Test.Public.Api.Fixtures;

using System.Reflection;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Framework.Helpers;
using EventStore;
using global::AssociationRegistry.Framework;
using global::AssociationRegistry.Public.Api;
using global::AssociationRegistry.Public.Api.Infrastructure.Extensions;
using global::AssociationRegistry.Public.ProjectionHost.Projections.Search;
using Marten;
using Marten.Events.Daemon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using NodaTime.Extensions;
using Xunit;
using Xunit.Sdk;
using IEvent = global::AssociationRegistry.Framework.IEvent;
using ProjectionHostProgram = AssociationRegistry.Public.ProjectionHost.Program;
using PublicApiProgram = AssociationRegistry.Public.Api.Program;

public class PublicApiFixture : IDisposable, IAsyncLifetime
{
    private readonly TestcontainerDatabase _postgreSqlContainer;

    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "p_";

    private IElasticClient _elasticClient = null!;
    private IDocumentStore _documentStore = null!;
    private TestServer _publicApiServer = null!;
    private IProjectionDaemon _daemon = null!;

    //public HttpClient HttpClient { get; }
    public PublicApiClient PublicApiClient
        => new(_publicApiServer.CreateClient());

    private string VerenigingenIndexName
        => _identifier;

    protected PublicApiFixture(string identifier)
    {
        _identifier += identifier.ToLowerInvariant();

        _postgreSqlContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(
                new PostgreSqlTestcontainerConfiguration()
                {
                    Database = identifier,
                    Username = "username",
                    Password = "Password",
                })
            .Build();
    }


    private IConfigurationRoot GetConfiguration(TestcontainerDatabase postgreSqlContainer)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(GetRootDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);
        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] = postgreSqlContainer.Database;
        tempConfiguration["PostgreSQLOptions:username"] = postgreSqlContainer.Username;
        tempConfiguration["PostgreSQLOptions:password"] = postgreSqlContainer.Password;
        tempConfiguration["PostgreSQLOptions:host"] = postgreSqlContainer.Hostname + ":" + postgreSqlContainer.Port;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _identifier;
        return tempConfiguration;
    }

    private static string GetRootDirectory()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");
        return rootDirectory;
    }

    private static void ConfigureBrolFeeder(IServiceProvider projectionServices)
        => projectionServices.GetRequiredService<IVerenigingBrolFeeder>().SetStatic();

    protected async Task AddEvent(string vCode, IEvent eventToAdd, CommandMetadata? metadata = null)
    {
        if (_documentStore is not { })
            throw new NullException("DocumentStore cannot be null when adding an event");

        if (_elasticClient is not { })
            throw new NullException("Elastic client cannot be null when adding an event");

        if (_daemon is not { })
            throw new NullException("Projection daemon cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode, new DateTime(2022, 1, 1).ToUniversalTime().ToInstant());

        var eventStore = new EventStore(_documentStore);
        await eventStore.Save(vCode, metadata, eventToAdd);

        await _daemon.WaitForNonStaleData(TimeSpan.FromSeconds(60));
        await _elasticClient.Indices.RefreshAsync(Indices.All);
    }

    private TestServer ConfigurePublicApiTestServer(TestcontainerDatabase postgreSqlContainer)
    {
        IWebHostBuilder hostBuilder = new WebHostBuilder();
        hostBuilder.UseConfiguration(GetConfiguration(postgreSqlContainer));
        hostBuilder.UseSetting("PostgreSQLOptions:database", postgreSqlContainer.Database);
        hostBuilder.UseSetting("PostgreSQLOptions:username", postgreSqlContainer.Username);
        hostBuilder.UseSetting("PostgreSQLOptions:password", postgreSqlContainer.Password);
        hostBuilder.UseSetting("PostgreSQLOptions:host", postgreSqlContainer.Hostname + ":" + postgreSqlContainer.Port);
        hostBuilder.UseStartup<Startup>();
        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
        hostBuilder.UseTestServer();
        return new TestServer(hostBuilder);
    }

    private IServiceProvider RunProjectionHost(TestcontainerDatabase postgreSqlContainer)
    {
        var webApplicationFactory = new WebApplicationFactory<ProjectionHostProgram>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting("PostgreSQLOptions:database", postgreSqlContainer.Database);
                    builder.UseSetting("PostgreSQLOptions:username", postgreSqlContainer.Username);
                    builder.UseSetting("PostgreSQLOptions:password", postgreSqlContainer.Password);
                    builder.UseSetting("PostgreSQLOptions:host", postgreSqlContainer.Hostname + ":" + postgreSqlContainer.Port);
                    builder.UseConfiguration(GetConfiguration(postgreSqlContainer));
                    builder.UseSetting("ElasticClientOptions:Indices:Verenigingen", _identifier);
                });

        return webApplicationFactory.Services;
    }

    private static IElasticClient CreateElasticClient(TestServer testServer)
        => (IElasticClient)testServer.Services.GetRequiredService(typeof(ElasticClient));

    private static void ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
    {
        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.CreateVerenigingIndex(verenigingenIndexName);

        client.Indices.Refresh(Indices.All);
    }

    // private void CreateDatabase()
    // {
    //     DocumentStore.For(
    //         opts =>
    //         {
    //             var connectionString = GetConnectionString(_configurationRoot, _configurationRoot["PostgreSQLOptions:database"]);
    //             var rootConnectionString = GetConnectionString(_configurationRoot, RootDatabase);
    //             opts.Connection(connectionString);
    //             opts.RetryPolicy(DefaultRetryPolicy.Times(5, _ => true, i => TimeSpan.FromSeconds(i)));
    //             opts.CreateDatabasesForTenants(
    //                 c =>
    //                 {
    //                     c.MaintenanceDatabase(rootConnectionString);
    //                     c.ForTenant()
    //                         .CheckAgainstPgDatabase()
    //                         .WithOwner(_configurationRoot["PostgreSQLOptions:username"]);
    //                 });
    //             opts.Events.StreamIdentity = StreamIdentity.AsString;
    //         });
    // }

    // private void DropDatabase()
    // {
    //     using var connection = new NpgsqlConnection(GetConnectionString(_configurationRoot, RootDatabase));
    //     using var cmd = connection.CreateCommand();
    //     try
    //     {
    //         connection.Open();
    //         // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
    //         cmd.CommandText += $"DROP DATABASE IF EXISTS {_configurationRoot["PostgreSQLOptions:database"]} WITH (FORCE);";
    //         cmd.ExecuteNonQuery();
    //     }
    //     finally
    //     {
    //         connection.Close();
    //         connection.Dispose();
    //     }
    // }

    private static string GetConnectionString(IConfiguration configurationRoot, string database)
        => $"host={configurationRoot["PostgreSQLOptions:host"]};" +
           $"database={database};" +
           $"password={configurationRoot["PostgreSQLOptions:password"]};" +
           $"username={configurationRoot["PostgreSQLOptions:username"]}";

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        PublicApiClient.Dispose();
        _publicApiServer.Dispose();

        _elasticClient.Indices.Delete(VerenigingenIndexName);
        _elasticClient.Indices.Refresh(Indices.All);
    }

    public virtual async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        _publicApiServer = ConfigurePublicApiTestServer(_postgreSqlContainer);

        _elasticClient = CreateElasticClient(_publicApiServer);
        WaitFor.ElasticSearchToBecomeAvailable(_elasticClient, _publicApiServer.Services.GetRequiredService<ILogger<PublicApiFixture>>())
            .GetAwaiter().GetResult();

        var projectionServices = RunProjectionHost(_postgreSqlContainer);
        _documentStore = projectionServices.GetRequiredService<IDocumentStore>();
        _daemon = _documentStore.BuildProjectionDaemonAsync().GetAwaiter().GetResult();
        _daemon.StartAllShards().GetAwaiter().GetResult();

        ConfigureBrolFeeder(projectionServices);

        ConfigureElasticClient(_elasticClient, VerenigingenIndexName);
    }

    public virtual async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }
}

// Implement IRetryPolicy interface
