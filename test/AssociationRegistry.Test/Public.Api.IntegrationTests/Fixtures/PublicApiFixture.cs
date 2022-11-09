﻿namespace AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.Extensions;
using AssociationRegistry.Public.Api.Infrastructure;
using AssociationRegistry.Public.Api.Projections;
using Framework.Helpers;
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
using IEvent = Events.IEvent;

public class PublicApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier;
    private readonly IConfigurationRoot _configurationRoot;

    private HttpClient? _httpClient;
    private IElasticClient? _elasticClient;
    private DocumentStore? _documentStore;
    private TestServer? _testServer;

    public HttpClient HttpClient
        => _httpClient!;

    private string VerenigingenIndexName
        => _configurationRoot["ElasticClientOptions:Indices:Verenigingen"];

    protected PublicApiFixture(string identifier)
    {
        _identifier += "_" + identifier.ToLowerInvariant();
        _configurationRoot = GetConfiguration();
    }

    public virtual async Task InitializeAsync()
    {
        await WaitFor.PostGreSQLToBecomeAvailable(
            LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitForPostgresTestLogger"),
            GetConnectionString(_configurationRoot, RootDatabase)
        );

        _testServer = ConfigureTestServer();

        ConfigureBrolFeeder(_testServer);

        _httpClient = _testServer.CreateClient();
        _elasticClient = CreateElasticClient(_testServer);
        _documentStore = ConfigureDocumentStore(_testServer);

        await WaitFor.ElasticSearchToBecomeAvailable(_elasticClient, LoggerFactory.Create(opt => opt.AddConsole()).CreateLogger("waitForElasticSearchTestLogger"));
        ConfigureElasticClient(_elasticClient, VerenigingenIndexName);
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
        tempConfiguration["PostgreSQLOptions:database"] += _identifier;
        tempConfiguration["ElasticClientOptions:Indices:Verenigingen"] += _identifier;
        return tempConfiguration;
    }

    private static void ConfigureBrolFeeder(TestServer testServer)
        => testServer.Services.GetRequiredService<IVerenigingBrolFeeder>().SetStatic();

    protected async Task AddEvent(string vCode, IEvent eventToAdd)
    {
        if (_documentStore is not { })
            throw new NullException("DocumentStore cannot be null when adding an event");

        if (_elasticClient is not { })
            throw new NullException("Elastic client cannot be null when adding an event");

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

        client.Indices.CreateVerenigingIndex(verenigingenIndexName);

        client.Indices.Refresh(Indices.All);
    }

    private DocumentStore ConfigureDocumentStore(TestServer testServer)
    {
        return DocumentStore.For(
            opts =>
            {
                var connectionString = GetConnectionString(_configurationRoot, _configurationRoot["PostgreSQLOptions:database"]);
                var rootConnectionString = GetConnectionString(_configurationRoot, RootDatabase);

                opts.Connection(connectionString);

                opts.CreateDatabasesForTenants(
                    c =>
                    {
                        c.MaintenanceDatabase(rootConnectionString);
                        c.ForTenant()
                            .CheckAgainstPgDatabase()
                            .WithOwner(_configurationRoot["PostgreSQLOptions:username"]);
                    });

                opts.Serializer(MartenExtentions.CreateCustomMartenSerializer());

                opts.Events.StreamIdentity = StreamIdentity.AsString;

                opts.Projections.Add(
                    new MartenSubscription(
                        new MartenEventsConsumer(testServer.Services)),
                    ProjectionLifecycle.Async);

                opts.AddPostgresProjections();
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

    public Task DisposeAsync()
        => Task.CompletedTask;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _httpClient?.Dispose();
        _testServer?.Dispose();
        _documentStore?.Dispose();

        DropDatabase();

        _elasticClient?.Indices.Delete(VerenigingenIndexName);
        _elasticClient?.Indices.Refresh(Indices.All);
    }
}
