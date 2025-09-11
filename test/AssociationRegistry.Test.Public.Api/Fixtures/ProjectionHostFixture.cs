namespace AssociationRegistry.Test.Public.Api.Fixtures;

using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using EventStore;
using Framework.Helpers;
using Common.Database;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Elastic.Clients.Elasticsearch;
using Npgsql;
using System.Reflection;
using Xunit;
using ProjectionHostProgram = AssociationRegistry.Public.ProjectionHost.Program;

public class ProjectionHostFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "p_";
    private readonly IConfigurationRoot _configurationRoot;
    public IDocumentStore DocumentStore { get; }
    public WebApplicationFactory<ProjectionHostProgram> ProjectionHost { get; }
    private readonly ElasticsearchClient _elasticClient;

    private string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"]!;

    protected ProjectionHostFixture(string identifier)
    {
        _identifier += identifier.ToLowerInvariant();
        _configurationRoot = GetConfiguration();

        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<ProjectionHostFixture>(), GetConnectionString(_configurationRoot, RootDatabase))
               .GetAwaiter().GetResult();

        DropDatabase();
        CreateDatabaseFromTemplate();

        ProjectionHost = RunProjectionHost();
        _elasticClient = CreateElasticClient(ProjectionHost.Services);

        WaitFor.ElasticSearchToBecomeAvailable(_elasticClient, ProjectionHost.Services.GetRequiredService<ILogger<ProjectionHostFixture>>())
               .GetAwaiter().GetResult();

        ConfigureElasticClient(_elasticClient, VerenigingenIndexName)
           .GetAwaiter().GetResult();

        DocumentStore = ProjectionHost.Services.GetRequiredService<IDocumentStore>();
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

    private static ElasticsearchClient CreateElasticClient(IServiceProvider services)
        => (ElasticsearchClient)services.GetRequiredService(typeof(ElasticsearchClient));

    private static async Task ConfigureElasticClient(ElasticsearchClient client, string verenigingenIndexName)
    {
        if ((await client.Indices.ExistsAsync(verenigingenIndexName)).Exists)
            await client.Indices.DeleteAsync(verenigingenIndexName);

        var verenigingIndex = await client.CreateVerenigingIndexAsync(verenigingenIndexName);

        if (!verenigingIndex.IsValidResponse)
            throw new Exception($"Indexing failed: {verenigingIndex.DebugInformation}");

        await client.Indices.RefreshAsync(Indices.All);
    }

    private void CreateDatabaseFromTemplate()
    {
        DatabaseTemplateHelper.CreateDatabaseFromTemplate(
            _configurationRoot, 
            _configurationRoot["PostgreSQLOptions:database"]!, 
            new NullLogger<ProjectionHostFixture>());
    }

    private void DropDatabase()
    {
        using var connection = new NpgsqlConnection(GetConnectionString(_configurationRoot, RootDatabase));
        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();
            // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
            cmd.CommandText += $"DROP DATABASE IF EXISTS \"{_configurationRoot["PostgreSQLOptions:database"]}\" WITH (FORCE);";
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
