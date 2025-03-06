namespace AssociationRegistry.Test.Public.Api.Fixtures;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using Events;
using EventStore;
using Framework.Helpers;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;
using NodaTime.Extensions;
using Npgsql;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using ProjectionHostProgram = AssociationRegistry.Public.ProjectionHost.Program;

public class ProjectionHostFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "p_";
    private readonly IConfigurationRoot _configurationRoot;
    public IDocumentStore DocumentStore { get; }
    public EventConflictResolver EventConflictResolver { get; }
    public WebApplicationFactory<ProjectionHostProgram> ProjectionHost { get; }
    private readonly IElasticClient _elasticClient;

    private string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"]!;

    protected ProjectionHostFixture(string identifier)
    {
        _identifier += identifier.ToLowerInvariant();
        _configurationRoot = GetConfiguration();

        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<ProjectionHostFixture>(), GetConnectionString(_configurationRoot, RootDatabase))
               .GetAwaiter().GetResult();

        DropDatabase();
        CreateDatabase();

        ProjectionHost = RunProjectionHost();
        _elasticClient = CreateElasticClient(ProjectionHost.Services);

        WaitFor.ElasticSearchToBecomeAvailable(_elasticClient, ProjectionHost.Services.GetRequiredService<ILogger<ProjectionHostFixture>>())
               .GetAwaiter().GetResult();

        ConfigureElasticClient(_elasticClient, VerenigingenIndexName)
           .GetAwaiter().GetResult();

        DocumentStore = ProjectionHost.Services.GetRequiredService<IDocumentStore>();
        EventConflictResolver = ProjectionHost.Services.GetRequiredService<EventConflictResolver>();
    }

    private IConfigurationRoot GetConfiguration()
    {
        var maybeRootDirectory = Directory
                                .GetParent(typeof(ProjectionHostProgram).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        var builder = new ConfigurationBuilder()
                     .SetBasePath(rootDirectory)
                     .AddJsonFile(path: "appsettings.json", optional: true)
                     .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var tempConfiguration = builder.Build();
        tempConfiguration["PostgreSQLOptions:database"] = _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] = _identifier;

        return tempConfiguration;
    }

    protected async Task AddEvent(string vCode, IEvent eventToAdd, CommandMetadata? metadata = null)
    {
        if (DocumentStore is null)
            throw new ArgumentNullException("DocumentStore cannot be null when adding an event");

        if (_elasticClient is null)
            throw new ArgumentNullException("Elastic client cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode, new DateTime(year: 2022, month: 1, day: 1).ToUniversalTime().ToInstant(), Guid.NewGuid());

        var eventStore = new EventStore(DocumentStore, EventConflictResolver, NullLogger<EventStore>.Instance);
        await eventStore.Save(vCode, metadata, CancellationToken.None, eventToAdd);

        using var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();
        await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(60));

        // Make sure all documents are properly indexed
        await _elasticClient.Indices.RefreshAsync(Indices.All);
    }

    private WebApplicationFactory<ProjectionHostProgram> RunProjectionHost()
    {
        return new WebApplicationFactory<ProjectionHostProgram>()
           .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting(key: "PostgreSQLOptions:database", _identifier);
                    builder.UseConfiguration(_configurationRoot);
                    builder.UseSetting(key: "ElasticClientOptions:Indices:Verenigingen", _identifier);
                });
    }

    private static IElasticClient CreateElasticClient(IServiceProvider services)
        => (IElasticClient)services.GetRequiredService(typeof(ElasticClient));

    private static async Task ConfigureElasticClient(IElasticClient client, string verenigingenIndexName)
    {
        if ((await client.Indices.ExistsAsync(verenigingenIndexName)).Exists)
            await client.Indices.DeleteAsync(verenigingenIndexName);

        var verenigingIndex = await client.Indices.CreateVerenigingIndexAsync(verenigingenIndexName);

        if (!verenigingIndex.IsValid)
            throw verenigingIndex.OriginalException;

        await client.Indices.RefreshAsync(Indices.All);
    }

    private void CreateDatabase()
    {
        var configuration = GetConfiguration();
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

    public virtual ValueTask InitializeAsync()
        => ValueTask.CompletedTask;

    public virtual ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}

// Implement IRetryPolicy interface
