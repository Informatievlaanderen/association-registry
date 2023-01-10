namespace AssociationRegistry.Test.Public.Api.Fixtures;

using System.Reflection;
using Framework.Helpers;
using EventStore;
using global::AssociationRegistry.Framework;
using global::AssociationRegistry.Public.Api;
using global::AssociationRegistry.Public.Api.Infrastructure.Extensions;
using global::AssociationRegistry.Public.ProjectionHost.Projections.Search;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using NodaTime.Extensions;
using Npgsql;
using Xunit;
using Xunit.Sdk;
using IEvent = global::AssociationRegistry.Framework.IEvent;
using ProjectionHostProgram = AssociationRegistry.Public.ProjectionHost.Program;
using PublicApiProgram = AssociationRegistry.Public.Api.Program;

public class ProjectionHostFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "p_";
    private readonly IConfigurationRoot _configurationRoot;

    public IDocumentStore DocumentStore { get; }
    public WebApplicationFactory<ProjectionHostProgram> ProjectionHost { get; }
    private readonly IElasticClient _elasticClient;


    private string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"];

    protected ProjectionHostFixture(string identifier)
    {
        _identifier += identifier.ToLowerInvariant();
        _configurationRoot = GetConfiguration();

        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<PublicApiFixture>(), GetConnectionString(_configurationRoot, RootDatabase))
            .GetAwaiter().GetResult();
        CreateDatabase();

        ProjectionHost = RunProjectionHost();
        _elasticClient = CreateElasticClient(ProjectionHost.Services);

        WaitFor.ElasticSearchToBecomeAvailable(_elasticClient, ProjectionHost.Services.GetRequiredService<ILogger<ProjectionHostFixture>>())
            .GetAwaiter().GetResult();

        DocumentStore = ProjectionHost.Services.GetRequiredService<IDocumentStore>();
        ConfigureBrolFeeder(ProjectionHost.Services);
    }


    private IConfigurationRoot GetConfiguration()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        var builder = new ConfigurationBuilder()
            .SetBasePath(rootDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);
        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] = _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _identifier;
        return tempConfiguration;
    }

    private static void ConfigureBrolFeeder(IServiceProvider projectionServices)
        => projectionServices.GetRequiredService<IVerenigingBrolFeeder>().SetStatic();

    protected async Task AddEvent(string vCode, IEvent eventToAdd, CommandMetadata? metadata = null)
    {
        if (DocumentStore is not { })
            throw new NullException("DocumentStore cannot be null when adding an event");

        if (_elasticClient is not { })
            throw new NullException("Elastic client cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode, new DateTime(2022, 1, 1).ToUniversalTime().ToInstant());

        var eventStore = new EventStore(DocumentStore);
        await eventStore.Save(vCode, metadata, eventToAdd);

        var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();
        await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(60));

        // Make sure all documents are properly indexed
        await _elasticClient.Indices.RefreshAsync(Indices.All);
    }

    private TestServer ConfigurePublicApiTestServer()
    {
        IWebHostBuilder hostBuilder = new WebHostBuilder();
        hostBuilder.UseConfiguration(_configurationRoot);
        hostBuilder.UseStartup<Startup>();
        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
        hostBuilder.UseTestServer();
        return new TestServer(hostBuilder);
    }

    private WebApplicationFactory<ProjectionHostProgram> RunProjectionHost()
    {
        return new WebApplicationFactory<ProjectionHostProgram>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting("PostgreSQLOptions:database", _identifier);
                    builder.UseConfiguration(_configurationRoot);
                    builder.UseSetting("ElasticClientOptions:Indices:Verenigingen", _identifier);
                });
    }

    private static IElasticClient CreateElasticClient(IServiceProvider services)
        => (IElasticClient)services.GetRequiredService(typeof(ElasticClient));

    private static void ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
    {
        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.CreateVerenigingIndex(verenigingenIndexName);

        client.Indices.Refresh(Indices.All);
    }

    private void CreateDatabase()
    {
        Marten.DocumentStore.For(
            opts =>
            {
                var connectionString = GetConnectionString(_configurationRoot, _configurationRoot["PostgreSQLOptions:database"]);
                var rootConnectionString = GetConnectionString(_configurationRoot, RootDatabase);
                opts.Connection(connectionString);
                opts.RetryPolicy(DefaultRetryPolicy.Times(5, _ => true, i => TimeSpan.FromSeconds(i)));
                opts.CreateDatabasesForTenants(
                    c =>
                    {
                        c.MaintenanceDatabase(rootConnectionString);
                        c.ForTenant()
                            .CheckAgainstPgDatabase()
                            .WithOwner(_configurationRoot["PostgreSQLOptions:username"]);
                    });
                opts.Events.StreamIdentity = StreamIdentity.AsString;
            });
    }

    private void DropDatabase()
    {
        using var connection = new NpgsqlConnection(GetConnectionString(_configurationRoot, RootDatabase));
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

    private static string GetConnectionString(IConfiguration configurationRoot, string database)
        => $"host={configurationRoot["PostgreSQLOptions:host"]};" +
           $"database={database};" +
           $"password={configurationRoot["PostgreSQLOptions:password"]};" +
           $"username={configurationRoot["PostgreSQLOptions:username"]}";

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        DropDatabase();

        _elasticClient.Indices.Delete(VerenigingenIndexName);
        _elasticClient.Indices.Refresh(Indices.All);

        ProjectionHost.Dispose();
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    public virtual Task DisposeAsync()
        => Task.CompletedTask;
}

// Implement IRetryPolicy interface
