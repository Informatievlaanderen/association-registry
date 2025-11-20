namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using Admin.Api;
using Admin.Api.Infrastructure.Extensions;
using Admin.ProjectionHost.Projections;
using Alba;
using AlbaHost;
using Amazon.SQS;
using AssociationRegistry.Framework;
using Common.Clients;
using Common.Database;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Grar.NutsLau;
using Hosts.Configuration;
using IdentityModel.AspNetCore.OAuth2Introspection;
using JasperFx.CommandLine;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Events.Daemon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Elastic.Clients.Elasticsearch;
using EventStore.ConflictResolution;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using NodaTime;
using NodaTime.Text;
using Npgsql;
using Oakton;
using Scenarios.Givens.FeitelijkeVereniging;
using System.Diagnostics;
using TestClasses;
using Vereniging;
using Wolverine;
using Xunit;
using ProjectionHostProgram = Public.ProjectionHost.Program;

public class FullBlownApiSetup : IAsyncLifetime, IApiSetup, IDisposable
{
    public FullBlownApiSetup()
    {
    }

    public string? AuthCookie { get; private set; }
    public ILogger<Program> Logger { get; private set; }
    public IAlbaHost AdminApiHost { get; private set; }
    public IAlbaHost AcmApiHost { get; private set; }
    public IAlbaHost AdminProjectionHost { get; private set; }
    public IAlbaHost PublicProjectionHost { get; private set; }
    public IAlbaHost PublicApiHost { get; private set; }

    public IDocumentSession AdminApiSharedSession { get; private set; }

    public async ValueTask InitializeAsync()
    {
        SetUpAdminApiConfiguration();

        // Ensure database exists before starting any hosts
        await EnsureDatabaseExistsBeforeHostStarts("adminapi");

        JasperFxEnvironment.AutoStartHost = true;

        var adminApiHost = await AlbaHost.For<Program>(ConfigureForTesting("adminapi"));

        var clients = new Clients(adminApiHost.Services.GetRequiredService<OAuth2IntrospectionOptions>(),
                                  createClientFunc: () => new HttpClient());

        SuperAdminHttpClient = clients.SuperAdmin.HttpClient;
        UnautenticatedClient = clients.Unauthenticated.HttpClient;
        UnauthorizedClient = clients.Unauthorized.HttpClient;

        AdminApiHost = adminApiHost.EnsureEachCallIsAuthenticated(clients.Authenticated.HttpClient);
        AdminHttpClient = clients.Authenticated.HttpClient;

        await AdminApiHost.ResetAllMartenDataAsync();

        AdminApiSharedSession = AdminApiHost.DocumentStore().LightweightSession();

        var elasticSearchOptions = AdminApiHost.Server.Services.GetRequiredService<IConfiguration>().GetElasticSearchOptionsSection();
        ElasticClient = ElasticSearchExtensions.CreateElasticClient(elasticSearchOptions, NullLogger.Instance);
        await ElasticClient.Indices.DeleteAsync(elasticSearchOptions.Indices.DuplicateDetection);

        await InsertNutsLauInfo();

        AdminProjectionHost = await AlbaHost.For<Admin.ProjectionHost.Program>(
            ConfigureForTesting("adminproj"));

        Logger = AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

        PublicProjectionHost = await AlbaHost.For<ProjectionHostProgram>(
            ConfigureForTesting("publicproj"));

        PublicApiHost = await AlbaHost.For<Public.Api.Program>(
            ConfigureForTesting("publicapi"));

        AcmApiHost = (await AlbaHost.For<Acm.Api.Program>(
                ConfigureForTesting("acmapi")))
           .EnsureEachCallIsAuthenticatedForAcmApi();

        using var scope = AdminApiHost.Services.CreateScope();
        _serviceProvider = scope.ServiceProvider;

        MessageBus = _serviceProvider.GetRequiredService<IMessageBus>();
        VCodeService = _serviceProvider.GetRequiredService<IVCodeService>();

        ElasticClient = _serviceProvider.GetRequiredService<ElasticsearchClient>();
        await AdminApiHost.DocumentStore().Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        await using var session = PublicApiHost.DocumentStore().LightweightSession();

        // await ExecuteGiven(new RandomEventSequenceScenario());
        // await ExecuteGiven(new MassiveRandomEventSequenceScenario());

        await session.SaveChangesAsync();

        await AdminProjectionHost.StartAsync();
        await PublicProjectionHost.StartAsync();

    }

    public IProjectionDaemon AcmProjectionDaemon { get; set; }
    public IProjectionDaemon PublicProjectionDaemon { get; set; }
    public IVCodeService VCodeService { get; set; }

    private async Task InsertNutsLauInfo()
    {
        var documentStore = AdminApiHost.DocumentStore();

        await using var session = documentStore.LightweightSession();

        session.StoreObjects(NutsLauInfoMock.All);
        await session.SaveChangesAsync();
    }

    public IProjectionDaemon AdminProjectionDaemon { get; private set; }
    public ElasticsearchClient ElasticClient { get; set; }
    public HttpClient SuperAdminHttpClient { get; private set; }
    public HttpClient UnautenticatedClient { get; private set; }
    public HttpClient UnauthorizedClient { get; private set; }
    public HttpClient AdminHttpClient { get; private set; }

    private void SetUpAdminApiConfiguration()
    {
        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.development.json", false)
                           .AddJsonFile("appsettings.e2e.json", false)
                           .AddJsonFile($"appsettings.e2e.adminapi.json", false)
                           .Build();

        AdminApiConfiguration = configuration;
    }

    public IAmazonSQS AmazonSqs { get; set; }
    public IMessageBus MessageBus { get; set; }

    private Action<IWebHostBuilder> ConfigureForTesting(string name)
    {
        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.development.json", false)
                           .AddJsonFile("appsettings.e2e.json", false)
                           .AddJsonFile($"appsettings.e2e.{name}.json", false)
                           .Build();

        return b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());

            b.UseConfiguration(configuration);

            b.ConfigureServices((context, services) =>
              {
                  context.HostingEnvironment.EnvironmentName = "Development";
              })
             .UseSetting(key: "ASPNETCORE_ENVIRONMENT", value: "Development")
             .UseSetting(key: "ApplyAllDatabaseChangesDisabled", value: "true");
        };
    }

    private async Task EnsureDatabaseExistsBeforeHostStarts(string name)
    {
        // Only need to ensure DB exists for adminapi (primary host)
        if (name != "adminapi")
            return;

        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.development.json", false)
                           .AddJsonFile("appsettings.e2e.json", false)
                           .AddJsonFile($"appsettings.e2e.adminapi.json", false)
                           .Build();

        var databaseName = configuration["PostgreSQLOptions:database"] ?? configuration["PostgreSQLOptions:Database"];
        var connectionString = GetConnectionString(configuration, "postgres");

        try
        {
            // Check if database exists
            using var connection = new NpgsqlConnection(connectionString);
            using var cmd = connection.CreateCommand();

            connection.Open();
            cmd.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'";
            var exists = cmd.ExecuteScalar() != null;

            if (!exists)
            {
                Console.WriteLine($"Database '{databaseName}' does not exist. Creating from template...");

                // Check if golden master template is available
                if (DatabaseTemplateHelper.IsGoldenMasterTemplateAvailable(configuration, NullLogger.Instance))
                {
                    DatabaseTemplateHelper.CreateDatabaseFromTemplate(
                        configuration,
                        databaseName!,
                        NullLogger.Instance);
                    Console.WriteLine($"Database '{databaseName}' created successfully from template.");
                }
                else
                {
                    // Fallback to creating empty database
                    Console.WriteLine("Golden master template not available. Creating empty database.");
                    cmd.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Database '{databaseName}' created successfully.");
                }
            }
            else
            {
                Console.WriteLine($"Database '{databaseName}' already exists.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ensuring database exists: {ex.Message}");
            throw;
        }
    }

    private static string GetConnectionString(IConfiguration configuration, string database)
        => $"host={configuration["PostgreSQLOptions:host"]};" +
           $"database={database};" +
           $"password={configuration["PostgreSQLOptions:password"]};" +
           $"username={configuration["PostgreSQLOptions:username"]}";

    public IConfigurationRoot AdminApiConfiguration { get; set; }

    public async ValueTask DisposeAsync()
    {
        await AdminApiHost.StopAsync();
        await PublicApiHost.StopAsync();
        await AdminProjectionHost.StopAsync();
        await PublicProjectionHost.StopAsync();
        await AcmApiHost.StopAsync();
        await AdminApiHost.DisposeAsync();
        await PublicApiHost.DisposeAsync();
        await PublicProjectionHost.DisposeAsync();
        await AdminProjectionHost.DisposeAsync();
        await AcmApiHost.DisposeAsync();
    }

    public async Task<long> ExecuteGiven(IScenario scenario, IDocumentSession session)
    {
        session.SetHeader(MetadataHeaderNames.Initiator, value: "metadata.Initiator");
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
        session.CorrelationId = Guid.NewGuid().ToString();

        var givenEvents = await scenario.GivenEvents(AdminApiHost.Services.GetRequiredService<IVCodeService>());

        if (givenEvents.Length == 0)
            return 0;

        var eventConflictResolver = new EventConflictResolver(
            Array.Empty<IEventPreConflictResolutionStrategy>(),
            Array.Empty<IEventPostConflictResolutionStrategy>());

        var eventStore = new EventStore(session, eventConflictResolver, new PersoonsgegevensProcessor(new PersoonsgegevensEventTransformers(), new VertegenwoordigerPersoonsgegevensRepository(session, new VertegenwoordigerPersoonsgegevensQuery(session)), NullLogger<PersoonsgegevensProcessor>.Instance), NullLogger<EventStore>.Instance);

        long maxSequence = 0;
        foreach (var eventsPerStream in givenEvents)
        {
            var exists = await eventStore.Exists(VCode.Hydrate(eventsPerStream.Key));

            if (exists)
            {
                var vereniging = await eventStore.Load<VerenigingState>(VCode.Hydrate(eventsPerStream.Key), null);
                var streamAction = await eventStore.Save(eventsPerStream.Key, vereniging.Version,
                                                            new CommandMetadata("metadata.Initiator", new Instant(), Guid.NewGuid(), null), CancellationToken.None, eventsPerStream.Value);
                maxSequence = streamAction.Sequence.Value;
            }
            else
            {
                var streamAction = await eventStore.SaveNew(eventsPerStream.Key,
                                                            new CommandMetadata("metadata.Initiator", new Instant(), Guid.NewGuid(), null), CancellationToken.None, eventsPerStream.Value);
                maxSequence = streamAction.Sequence.Value;
            }
        }

        await session.SaveChangesAsync();

        return maxSequence;
    }

    public async Task RefreshIndices()
        => await ElasticClient.Indices.RefreshAsync(Indices.All);

    private readonly Dictionary<string, object> _ranContexts = new();
    private IServiceProvider _serviceProvider;

    public void Dispose()
    {
        AdminApiHost.Dispose();
        AcmApiHost.Dispose();
        AdminProjectionHost.Dispose();
        PublicProjectionHost.Dispose();
        PublicApiHost.Dispose();
        AdminProjectionDaemon.Dispose();
        SuperAdminHttpClient.Dispose();
        UnautenticatedClient.Dispose();
        UnauthorizedClient.Dispose();
        AdminHttpClient.Dispose();
        AmazonSqs.Dispose();
        AdminApiSharedSession.Dispose();
    }
}
