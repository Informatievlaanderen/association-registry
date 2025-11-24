namespace AssociationRegistry.Test.Acm.Api.Fixtures;

using AssociationRegistry.Acm.Api;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using Common.Database;
using Common.Fixtures;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore.ConflictResolution;
using Framework.Helpers;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Marten;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NodaTime;
using Npgsql;
using Oakton;
using Persoonsgegevens;
using Polly;
using System.Reflection;
using Vereniging;
using Xunit;
using ConfigurationExtensions = AssociationRegistry.Acm.Api.Infrastructure.Extensions.ConfigurationExtensions;

public abstract class AcmApiFixture : IDisposable, IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly string _identifier = "acmapifixture";
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public IDocumentStore DocumentStore
        => _webApplicationFactory.Services.GetRequiredService<IDocumentStore>();

    public EventConflictResolver EventConflictResolver
        => new(Array.Empty<IEventPreConflictResolutionStrategy>(), Array.Empty<IEventPostConflictResolutionStrategy>());

    public AcmApiClient AcmApiClient
        => AcmApiClients.Authenticated;

    public AcmApiClient UnauthenticatedClient
        => AcmApiClients.Unauthenticated;

    public IServiceProvider ServiceProvider
        => _webApplicationFactory.Services;

    protected AcmApiFixture()
    {
        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<AcmApiFixture>(),
                    GetConnectionString(GetConfiguration(), RootDatabase))
               .GetAwaiter().GetResult();

        CreateDatabaseFromTemplate(GetConfiguration());

        WaitFor.PostGreSQLToBecomeAvailable(
                    new NullLogger<AcmApiFixture>(),
                    GetConnectionString(GetConfiguration(), ConfigurationExtensions.GetPostgreSqlOptionsSection(GetConfiguration()).Database!))
               .GetAwaiter().GetResult();

        OaktonEnvironment.AutoStartHost = true;

        _webApplicationFactory = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.UseSetting($"{PostgreSqlOptionsSection.SectionName}:{nameof(PostgreSqlOptionsSection.Database)}", _identifier);

                    builder.UseConfiguration(GetConfiguration());

                    builder.ConfigureAppConfiguration(
                        cfg =>
                            cfg.SetBasePath(GetRootDirectoryOrThrow())
                               .AddJsonFile(path: "appsettings.json", optional: true)
                               .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                               .AddInMemoryCollection(
                                    new[]
                                    {
                                        new KeyValuePair<string, string>(
                                            $"{PostgreSqlOptionsSection.SectionName}:{nameof(PostgreSqlOptionsSection.Database)}",
                                            _identifier),
                                    })
                    );
                });

        var postgreSqlOptionsSection = _webApplicationFactory.Services.GetRequiredService<PostgreSqlOptionsSection>();

        WaitFor.PostGreSQLToBecomeAvailable(new NullLogger<AcmApiFixture>(), GetRootConnectionString(postgreSqlOptionsSection))
               .GetAwaiter().GetResult();

        AcmApiClients = new AcmApiClients(
            GetConfiguration().GetSection(nameof(OAuth2IntrospectionOptions))
                              .Get<OAuth2IntrospectionOptions>(),
            _webApplicationFactory.CreateClient);
    }

    public AcmApiClients AcmApiClients { get; }

    public AcmApiClient DefaultClient
        => AcmApiClients.Authenticated;

    public async ValueTask InitializeAsync()
        => await Given();

    public virtual async ValueTask DisposeAsync()
        => Dispose();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        DropDatabase();
    }

    private void CreateDatabaseFromTemplate(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection = ConfigurationExtensions.GetPostgreSqlOptionsSection(configuration);
        DatabaseTemplateHelper.CreateDatabaseFromTemplate(
            configuration,
            postgreSqlOptionsSection.Database!,
            new NullLogger<AcmApiFixture>());
    }

    private static string GetRootConnectionString(PostgreSqlOptionsSection postgreSqlOptionsSection)
        => $"host={postgreSqlOptionsSection.Host}:5432;" +
           "database=postgres;" +
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

        if (DocumentStore is null)
            throw new NullReferenceException("DocumentStore cannot be null when adding an event");

        using var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllAsync();

        if (daemon is null)
            throw new NullReferenceException("Projection daemon cannot be null when adding an event");

        metadata ??= new CommandMetadata(vCode.ToUpperInvariant(), new Instant(), Guid.NewGuid());
        await using var session = DocumentStore.LightweightSession();

        var eventStore = new EventStore(session, EventConflictResolver, new PersoonsgegevensProcessor(new PersoonsgegevensEventTransformers(), new VertegenwoordigerPersoonsgegevensRepository(session, new VertegenwoordigerPersoonsgegevensQuery(session)), NullLogger<PersoonsgegevensProcessor>.Instance), NullLogger<EventStore>.Instance);
        var result = StreamActionResult.Empty;


        await eventStore.SaveNew(VCode.Create(vCode.ToUpperInvariant()), metadata, CancellationToken.None, eventsToAdd);

        var retry = Policy
                   .Handle<Exception>()
                   .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: i => TimeSpan.FromSeconds(10 * i));

        await retry.ExecuteAsync(
            async () => { await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(value: 60)); });

        return result;
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

    protected abstract Task Given();
}
