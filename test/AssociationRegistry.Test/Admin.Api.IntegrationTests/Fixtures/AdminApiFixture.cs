namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using System.Reflection;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.EventStore;
using AssociationRegistry.Admin.Api.Verenigingen.VCodes;
using AssociationRegistry.Admin.Api.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Events;
using AssociationRegistry.Admin.Api.Extensions;
using AssociationRegistry.Framework;
using AssociationRegistry.Public.Api.Infrastructure.Extensions;
using Framework.Helpers;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Sdk;
using IEvent = AssociationRegistry.Framework.IEvent;

public class AdminApiFixture : IDisposable, IAsyncLifetime
{
    private readonly string _dbName = "a_";

    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public IDocumentStore DocumentStore
        => _webApplicationFactory.Services.GetRequiredService<IDocumentStore>();

    public AdminApiClient AdminApiClient
        => new(_webApplicationFactory.CreateClient());

    public IServiceProvider ServiceProvider
        => _webApplicationFactory.Services;

    protected AdminApiFixture(string dbName)
    {
        _dbName += dbName;
        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.ConfigureAppConfiguration(
                        cfg =>
                            cfg.SetBasePath(GetRootDirectoryOrThrow())
                                .AddJsonFile("appsettings.json", optional: true)
                                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                    );
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _dbName);
                    builder.ConfigureServices(
                        (context, _) =>
                        {
                            EnsureDbExists(context.Configuration.GetPostgreSqlOptionsSection());
                        });
                });
    }

    private static void EnsureDbExists(PostgreSqlOptionsSection postgreSqlOptionsSection)
    {
        Marten.DocumentStore.For(
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

    protected async Task<long> AddEvent(string vCode, IEvent eventToAdd, CommandMetadata metadata)
    {
        if (DocumentStore is not { })
            throw new NullException("DocumentStore cannot be null when adding an event");

        var eventStore = new EventStore(DocumentStore);
        var sequence = await eventStore.Save(vCode, metadata, eventToAdd);

        var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllShards();
        await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(20));

        return sequence;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        AdminApiClient.Dispose();
        _webApplicationFactory.Dispose();
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    public virtual Task DisposeAsync()
        => Task.CompletedTask;
}
