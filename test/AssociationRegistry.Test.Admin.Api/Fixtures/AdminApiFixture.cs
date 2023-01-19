namespace AssociationRegistry.Test.Admin.Api.Fixtures;

using System.Reflection;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Marten;
using Marten.Events.Daemon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using IEvent = global::AssociationRegistry.Framework.IEvent;

public class AdminApiFixture : IDisposable, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly PostgreSqlTestcontainer _pgContainer;
    private IProjectionDaemon _daemon;

    public IDocumentStore DocumentStore
        => _webApplicationFactory.Services.GetRequiredService<IDocumentStore>();

    public AdminApiClient AdminApiClient
        => new(_webApplicationFactory.CreateClient());

    public IServiceProvider ServiceProvider
        => _webApplicationFactory.Services;

    protected AdminApiFixture(string dbName)
    {
        _pgContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(
                new PostgreSqlTestcontainerConfiguration()
                {
                    Database = "a_" + dbName,
                    Username = "username",
                    Password = "password",
                })
            .WithCleanUp(true)
            .Build();
        _pgContainer.StartAsync().GetAwaiter().GetResult();

        _webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _pgContainer.Database);
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Host)}", _pgContainer.Hostname + ":" + _pgContainer.Port);
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Password)}", _pgContainer.Password);
                    builder.UseSetting($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Username)}", _pgContainer.Username);
                    builder.ConfigureAppConfiguration(
                        cfg =>
                            cfg.SetBasePath(GetRootDirectoryOrThrow())
                                .AddJsonFile("appsettings.json", optional: true)
                                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                                .AddInMemoryCollection(
                                    new[]
                                    {
                                        new KeyValuePair<string, string>($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Database)}", _pgContainer.Database),
                                        new KeyValuePair<string, string>($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Host)}", _pgContainer.Hostname + ":" + _pgContainer.Port),
                                        new KeyValuePair<string, string>($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Password)}", _pgContainer.Password),
                                        new KeyValuePair<string, string>($"{PostgreSqlOptionsSection.Name}:{nameof(PostgreSqlOptionsSection.Username)}", _pgContainer.Username),
                                    })
                    );
                });
        _daemon = DocumentStore.BuildProjectionDaemonAsync().GetAwaiter().GetResult();
        _daemon.StartAllShards().GetAwaiter().GetResult();
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

    protected async Task<SaveChangesResult> AddEvent(string vCode, IEvent eventToAdd, CommandMetadata metadata)
    {
        var eventStore = _webApplicationFactory.Services.GetRequiredService<IEventStore>();
        var result = await eventStore.Save(vCode.ToUpperInvariant(), metadata, eventToAdd);

        await _daemon.WaitForNonStaleData(TimeSpan.FromSeconds(60));

        return result;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _pgContainer.StopAsync().GetAwaiter().GetResult();
        AdminApiClient.Dispose();
        _webApplicationFactory.Dispose();
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    public virtual Task DisposeAsync()
        => Task.CompletedTask;
}
