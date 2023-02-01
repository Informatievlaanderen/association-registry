namespace AssociationRegistry.Test.Admin.Api.TakeTwo;

using System.Reflection;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using Framework.Helpers;
using Fixtures;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Npgsql;
using Polly;
using Xunit;
using IEvent = global::AssociationRegistry.Framework.IEvent;

public abstract class AdminApiFixture2 : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "adminApiFixture";

    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public IDocumentStore DocumentStore
        => _webApplicationFactory.Services.GetRequiredService<IDocumentStore>();

    public AdminApiClient AdminApiClient
        => new(_webApplicationFactory.CreateClient());

    public IServiceProvider ServiceProvider
        => _webApplicationFactory.Services;

    public AdminApiFixture2()
    {
        WaitFor.PostGreSQLToBecomeAvailable(
                new NullLogger<AdminApiFixture2>(),
                GetConnectionString(GetConfiguration(), RootDatabase))
            .GetAwaiter().GetResult();

        EnsureDbExists(GetConfiguration().GetPostgreSqlOptionsSection());

        WaitFor.PostGreSQLToBecomeAvailable(
                new NullLogger<AdminApiFixture2>(),
                GetConnectionString(GetConfiguration(), GetConfiguration().GetPostgreSqlOptionsSection().Database!))
            .GetAwaiter().GetResult();

        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier);
                    builder.ConfigureAppConfiguration(
                        cfg =>
                            cfg.SetBasePath(GetRootDirectoryOrThrow())
                                .AddJsonFile("appsettings.json", optional: true)
                                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                                .AddInMemoryCollection(new []
                                {
                                    new KeyValuePair<string, string>($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier),
                                })
                    );
                });
        var postgreSqlOptionsSection = _webApplicationFactory.Services.GetRequiredService<PostgreSqlOptionsSection>();
        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<AdminApiFixture2>(), GetRootConnectionString(postgreSqlOptionsSection))
            .GetAwaiter().GetResult();
    }

    private static void EnsureDbExists(PostgreSqlOptionsSection postgreSqlOptionsSection)
    {
        using var documentStore = Marten.DocumentStore.For(
            options =>
            {
                options.Connection(postgreSqlOptionsSection.GetConnectionString());
                options.CreateDatabasesForTenants(
                    databaseConfig =>
                    {
                        databaseConfig.MaintenanceDatabase(GetRootConnectionString(postgreSqlOptionsSection));
                        databaseConfig.ForTenant()
                            .CheckAgainstPgDatabase()
                            .WithOwner(postgreSqlOptionsSection.Username!);
                    });
                options.RetryPolicy(DefaultRetryPolicy.Times(5, _ => true, i => TimeSpan.FromSeconds(i)));
            });
    }

    private static string GetRootConnectionString(PostgreSqlOptionsSection postgreSqlOptionsSection)
        => $"host={postgreSqlOptionsSection.Host}:5432;" +
           $"database=postgres;" +
           $"password={postgreSqlOptionsSection.Password};" +
           $"username={postgreSqlOptionsSection.Username}";

    private static string GetRootDirectoryOrThrow()
    {
        var maybeRootDirectory = Directory
            .GetParent(Assembly.GetExecutingAssembly().Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");
        return rootDirectory;
    }

    protected async Task<StreamActionResult> AddEvents(string vCode, IEvent[] eventsToAdd, CommandMetadata? metadata = null)
    {
        if (!eventsToAdd.Any())
            return StreamActionResult.Empty;

        if (DocumentStore is not { })
            throw new NullReferenceException("DocumentStore cannot be null when adding an event");

        using var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();

        if (daemon is not { })
            throw new NullReferenceException("Projection daemon cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode.ToUpperInvariant(), new Instant());

        var eventStore = new EventStore(DocumentStore);
        var result = StreamActionResult.Empty;
        foreach (var @event in eventsToAdd)
        {
            result = await eventStore.Save(vCode.ToUpperInvariant(), metadata, @event);
        }

        var retry = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(10*i));

        await retry.ExecuteAsync(
            async () =>
            {
                await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(60));
            });

        return result;
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
        AdminApiClient.SafeDispose();
        _webApplicationFactory.SafeDispose();
        DropDatabase();
    }

    public async Task InitializeAsync()
    {
        await Given();
    }

    public virtual Task DisposeAsync()
        => Task.CompletedTask;

    protected abstract Task Given();
}
