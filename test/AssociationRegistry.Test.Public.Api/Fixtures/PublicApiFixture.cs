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
using Marten.Events.Daemon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
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

public class PublicApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "p_";

    private readonly WebApplicationFactory<PublicApiProgram> _publicApiServer;
    private readonly WebApplicationFactory<ProjectionHostProgram> _projectionHostServer;
    private readonly IProjectionDaemon _daemon;

    private IElasticClient ElasticClient
        => (IElasticClient)_publicApiServer.Services.GetRequiredService(typeof(ElasticClient));

    private IDocumentStore DocumentStore
        => _projectionHostServer.Services.GetRequiredService<IDocumentStore>();

    public PublicApiClient PublicApiClient
        => new(_publicApiServer.CreateClient());

    private string VerenigingenIndexName
        => GetConfiguration()["ElasticClientOptions:Indices:Verenigingen"];

    protected PublicApiFixture(string identifier)
    {
        _identifier += identifier.ToLowerInvariant();

        WaitFor.PostGreSQLToBecomeAvailable(
                new NullLogger<PublicApiFixture>(),
                GetConnectionString(GetConfiguration(), RootDatabase))
            .GetAwaiter().GetResult();

        CreateDatabase(GetConfiguration());

        _publicApiServer = new WebApplicationFactory<PublicApiProgram>()
            .WithWebHostBuilder(
                builder => { builder.UseConfiguration(GetConfiguration()); });

        WaitFor.ElasticSearchToBecomeAvailable(ElasticClient, _publicApiServer.Services.GetRequiredService<ILogger<PublicApiFixture>>())
            .GetAwaiter().GetResult();

        _projectionHostServer = new WebApplicationFactory<ProjectionHostProgram>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting("PostgreSQLOptions:database", _identifier);
                    builder.UseConfiguration(GetConfiguration());
                    builder.UseSetting("ElasticClientOptions:Indices:Verenigingen", _identifier);
                });

        _daemon = DocumentStore.BuildProjectionDaemonAsync().GetAwaiter().GetResult();
        _daemon.StartAllShards().GetAwaiter().GetResult();

        ConfigureBrolFeeder(_projectionHostServer.Services);

        ConfigureElasticClient(ElasticClient, VerenigingenIndexName);
    }


    private IConfigurationRoot GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(GetRootDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);
        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] = _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _identifier;
        return tempConfiguration;
    }

    private static string GetRootDirectory()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Program).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");
        return rootDirectory;
    }

    private static void ConfigureBrolFeeder(IServiceProvider projectionServices)
        => projectionServices.GetRequiredService<IVerenigingBrolFeeder>().SetStatic();

    protected async Task AddEvent(string vCode, IEvent eventToAdd, CommandMetadata? metadata = null)
    {
        if (DocumentStore is not { })
            throw new NullException("DocumentStore cannot be null when adding an event");

        if (ElasticClient is not { })
            throw new NullException("Elastic client cannot be null when adding an event");

        if (_daemon is not { })
            throw new NullException("Projection daemon cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode.ToUpperInvariant(), new DateTime(2022, 1, 1).ToUniversalTime().ToInstant());

        var eventStore = new EventStore(DocumentStore);
        await eventStore.Save(vCode, metadata, eventToAdd);

        await _daemon.WaitForNonStaleData(TimeSpan.FromSeconds(60));
        await ElasticClient.Indices.RefreshAsync(Indices.All);
    }

    private static void ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
    {
        if (client.Indices.Exists(verenigingenIndexName).Exists)
            client.Indices.Delete(verenigingenIndexName);

        client.Indices.CreateVerenigingIndex(verenigingenIndexName);

        client.Indices.Refresh(Indices.All);
    }

    private static void CreateDatabase(IConfiguration configuration)
    {
        Marten.DocumentStore.For(
            opts =>
            {
                var connectionString = GetConnectionString(configuration, configuration["PostgreSQLOptions:database"]);
                var rootConnectionString = GetConnectionString(configuration, RootDatabase);
                opts.Connection(connectionString);
                opts.RetryPolicy(DefaultRetryPolicy.Times(5, _ => true, i => TimeSpan.FromSeconds(i)));
                opts.CreateDatabasesForTenants(
                    c =>
                    {
                        c.MaintenanceDatabase(rootConnectionString);
                        c.ForTenant()
                            .CheckAgainstPgDatabase()
                            .WithOwner(configuration["PostgreSQLOptions:username"]);
                    });
                opts.Events.StreamIdentity = StreamIdentity.AsString;
            });
    }

    private void DropDatabase()
    {
        using var connection = new NpgsqlConnection(GetConnectionString(GetConfiguration(), RootDatabase));
        using var cmd = connection.CreateCommand();
        try
        {
            connection.Open();
            // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
            cmd.CommandText += $"DROP DATABASE IF EXISTS {GetConfiguration()["PostgreSQLOptions:database"]} WITH (FORCE);";
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
        PublicApiClient.Dispose();

        DropDatabase();

        ElasticClient.Indices.Delete(VerenigingenIndexName);
        ElasticClient.Indices.Refresh(Indices.All);

        _publicApiServer.Dispose();
        _daemon.Dispose();
        _projectionHostServer.Dispose();
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    public virtual Task DisposeAsync()
        => Task.CompletedTask;
}
