namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using Events;
using Framework.Helpers;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using NodaTime;
using Npgsql;
using Oakton;
using Polly;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Policy = Polly.Policy;
using ProjectionHostProgram = AssociationRegistry.Public.ProjectionHost.Program;
using PublicApiProgram = AssociationRegistry.Public.Api.Program;

public class PublicApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "publicapifixture";
    private readonly WebApplicationFactory<PublicApiProgram> _publicApiServer;
    private readonly WebApplicationFactory<ProjectionHostProgram> _projectionHostServer;

    public IElasticClient ElasticClient
        => (IElasticClient)_publicApiServer.Services.GetRequiredService(typeof(ElasticClient));

    private IDocumentStore ProjectionsDocumentStore
        => _projectionHostServer.Services.GetRequiredService<IDocumentStore>();

    private EventConflictResolver EventConflictResolver
        => _projectionHostServer.Services.GetRequiredService<EventConflictResolver>();

    public PublicApiClient PublicApiClient
        => new(_publicApiServer.CreateClient());

    private string VerenigingenIndexName
        => GetConfiguration()["ElasticClientOptions:Indices:Verenigingen"];

    public IServiceProvider ServiceProvider => _publicApiServer.Services;

    public PublicApiFixture()
    {
        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<PublicApiFixture>(),
                    GetConnectionString(GetConfiguration(), RootDatabase))
               .GetAwaiter().GetResult();

        DropDatabase();
        CreateDatabase(GetConfiguration());

        _publicApiServer = new WebApplicationFactory<PublicApiProgram>()
           .WithWebHostBuilder(
                builder => { builder.UseConfiguration(GetConfiguration()); });

        WaitFor.ElasticSearchToBecomeAvailable(ElasticClient, _publicApiServer.Services.GetRequiredService<ILogger<PublicApiFixture>>())
               .GetAwaiter().GetResult();

        OaktonEnvironment.AutoStartHost = true;

        _projectionHostServer = new WebApplicationFactory<ProjectionHostProgram>()
           .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting(key: "PostgreSQLOptions:database", _identifier);
                    builder.UseConfiguration(GetConfiguration());
                    builder.UseSetting(key: "ElasticClientOptions:Indices:Verenigingen", _identifier);
                });

        ConfigureElasticClient(ElasticClient, VerenigingenIndexName);
    }

    private IConfigurationRoot GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
                     .SetBasePath(GetRootDirectory())
                     .AddJsonFile(path: "appsettings.json", optional: true)
                     .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] = _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _identifier;

        return tempConfiguration;
    }

    private static string GetRootDirectory()
    {
        var maybeRootDirectory = Directory
                                .GetParent(typeof(PublicApiProgram).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        return rootDirectory;
    }

    protected async Task AddEvents(string vCode, IEvent[] eventsToAdd, CommandMetadata? metadata = null)
    {
        if (!eventsToAdd.Any())
            return;

        if (ProjectionsDocumentStore is null)
            throw new NullReferenceException("DocumentStore cannot be null when adding an event");

        if (ElasticClient is null)
            throw new NullReferenceException("Elastic client cannot be null when adding an event");

        using var daemon = await ProjectionsDocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();

        if (daemon is null)
            throw new NullReferenceException("Projection daemon cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode.ToUpperInvariant(), new Instant(), Guid.NewGuid());

        var eventStore = new EventStore(ProjectionsDocumentStore, EventConflictResolver, NullLogger<EventStore>.Instance);

        foreach (var @event in eventsToAdd)
        {
            await eventStore.Save(vCode, metadata, CancellationToken.None, @event);
        }

        var retry = Policy
                   .Handle<Exception>()
                   .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: i => TimeSpan.FromSeconds(10 * i));

        await retry.ExecuteAsync(
            async () =>
            {
                await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(60));
                await ElasticClient.Indices.RefreshAsync(Indices.All);
            });
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
        using var connection = new NpgsqlConnection(GetConnectionString(configuration, RootDatabase));
        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();

            cmd.CommandText +=
                $"CREATE DATABASE {configuration["PostgreSQLOptions:database"]} WITH OWNER = {configuration["PostgreSQLOptions:username"]};";

            cmd.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    private void DropDatabase()
    {
        using var connection = new NpgsqlConnection(GetConnectionString(GetConfiguration(), RootDatabase));
        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();
            // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
            cmd.CommandText += $"DROP DATABASE IF EXISTS \"{GetConfiguration()["PostgreSQLOptions:database"]}\" WITH (FORCE);";
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
        _projectionHostServer.Dispose();
    }

    public virtual ValueTask InitializeAsync()
        => ValueTask.CompletedTask;

    public virtual ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}
