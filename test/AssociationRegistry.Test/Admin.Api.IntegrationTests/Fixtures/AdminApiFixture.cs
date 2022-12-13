namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Events;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.VCodes;
using AssociationRegistry.Framework;
using AssociationRegistry.Public.Api.Extensions;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using VCodes;
using Weasel.Core;
using Xunit;
using Xunit.Sdk;
using IEvent = AssociationRegistry.Framework.IEvent;

public class AdminApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier;
    private readonly IConfigurationRoot _configurationRoot;

    private readonly TestServer _testServer;

    public DocumentStore DocumentStore { get; }
    public HttpClient HttpClient { get; }

    protected AdminApiFixture(string identifier)
    {
        _identifier += identifier.ToLowerInvariant();
        _configurationRoot = GetConfiguration();

        DocumentStore = ConfigureDocumentStore();
        _testServer = ConfigureTestServer();

        HttpClient = _testServer.CreateClient();
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
        return tempConfiguration;
    }

    protected async Task AddEvent(string vCode, IEvent eventToAdd, CommandMetadata metadata)
    {
        if (DocumentStore is not { })
            throw new NullException("DocumentStore cannot be null when adding an event");

        var eventStore = new EventStore(DocumentStore);
        await eventStore.Save(vCode, metadata, eventToAdd);

        var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();
        await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(10));
    }

    public async Task<string> Search(string uri)
    {
        var responseMessage = await GetResponseMessage(uri);
        return await responseMessage.Content.ReadAsStringAsync();
    }

    public async Task<HttpResponseMessage> GetResponseMessage(string uri)
    {
        if (HttpClient is null)
            throw new NullReferenceException("HttpClient needs to be set before performing a get");

        return await HttpClient.GetAsync(uri);
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

    private DocumentStore ConfigureDocumentStore()
    {
        var configureDocumentStore = DocumentStore.For(
            opts =>
            {
                var connectionString = GetConnectionString(_configurationRoot, _configurationRoot["PostgreSQLOptions:database"]);
                var rootConnectionString = GetRootConnectionString(_configurationRoot);

                opts.Connection(connectionString);

                opts.CreateDatabasesForTenants(
                    c =>
                    {
                        c.MaintenanceDatabase(rootConnectionString);
                        c.ForTenant()
                            .CheckAgainstPgDatabase()
                            .WithOwner(_configurationRoot["PostgreSQLOptions:username"]);
                    });

                opts.Events.StreamIdentity = StreamIdentity.AsString;
                opts.Storage.Add(new VCodeSequence(opts, VCode.StartingVCode));
                opts.Serializer(MartenExtensions.CreateCustomMartenSerializer());
                opts.Events.MetadataConfig.EnableAll();
                opts.AddPostgresProjections();
                opts.AutoCreateSchemaObjects = AutoCreate.All;
            });
        configureDocumentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
        return configureDocumentStore;
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

    private static string GetRootConnectionString(IConfiguration configurationRoot)
        => $"host={configurationRoot["RootPostgreSQLOptions:host"]};" +
           $"database={configurationRoot["RootPostgreSQLOptions:database"]};" +
           $"password={configurationRoot["RootPostgreSQLOptions:password"]};" +
           $"username={configurationRoot["RootPostgreSQLOptions:username"]}";

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        HttpClient.Dispose();
        _testServer.Dispose();
        DocumentStore.Dispose();

        DropDatabase();
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    public virtual Task DisposeAsync()
        => Task.CompletedTask;
}
