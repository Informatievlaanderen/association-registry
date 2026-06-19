namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using System.Diagnostics;
using Admin.Api;
using Admin.Api.Infrastructure.Extensions;
using Admin.ProjectionHost.Projections;
using Admin.ProjectionHost.Projections.Locaties;
using Admin.ProjectionHost.Projections.Search;
using Alba;
using AlbaHost;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using AssociationRegistry.Framework;
using Common.Clients;
using Common.Database;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Elastic.Clients.Elasticsearch;
using EventStore.ConflictResolution;
using Grar.NutsLau;
using Hosts.Configuration;
using IdentityModel.AspNetCore.OAuth2Introspection;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.CommandLine;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Events.Daemon;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Logging;
using MartenDb.Store;
using MartenDb.Subscriptions;
using MartenDb.Transformers;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using NodaTime.Text;
using Npgsql;
using Scenarios.Givens.FeitelijkeVereniging;
using TestClasses;
using Vereniging;
using Wolverine;
using Xunit;
using ProjectionHostProgram = Public.ProjectionHost.Program;

public class FullBlownApiSetup : IAsyncLifetime, IApiSetup, IDisposable
{
    public FullBlownApiSetup() { }

    private bool _disposed;

    public string? AuthCookie { get; private set; }
    public ILogger<Program> Logger { get; private set; }
    public IAlbaHost AdminApiHost { get; private set; }
    public IAlbaHost AcmApiHost { get; private set; }
    public IAlbaHost AdminProjectionHost { get; private set; }
    public IAlbaHost PublicProjectionHost { get; private set; }
    public IAlbaHost PublicApiHost { get; private set; }

    public async ValueTask InitializeAsync()
    {
        SetUpAdminApiConfiguration();

        ResetDatabaseBeforeHostStarts("adminapi");
        ApplyMartenSchemaChanges();
        await EnsureLocalS3BucketExists();

        JasperFxEnvironment.AutoStartHost = true;

        var adminApiHost = await AlbaHost.For<Program>(ConfigureForTesting<Program>("adminapi"));

        var clients = new Clients(
            adminApiHost.Services.GetRequiredService<OAuth2IntrospectionOptions>(),
            createClientFunc: () => new HttpClient()
        );

        SuperAdminHttpClient = clients.SuperAdmin.HttpClient;
        UnautenticatedClient = clients.Unauthenticated.HttpClient;
        UnauthorizedClient = clients.Unauthorized.HttpClient;

        AdminApiHost = adminApiHost.EnsureEachCallIsAuthenticated(clients.Authenticated.HttpClient);
        AdminHttpClient = clients.Authenticated.HttpClient;

        await AdminApiHost.ResetAllMartenDataAsync();

        var elasticSearchOptions = AdminApiHost
            .Server.Services.GetRequiredService<IConfiguration>()
            .GetElasticSearchOptionsSection();

        ElasticClient = ElasticSearchExtensions.CreateElasticClient(elasticSearchOptions, NullLogger.Instance);
        var publicElasticSearchOptions = BuildConfiguration("publicproj").GetElasticSearchOptionsSection();

        await ElasticClient.Indices.DeleteAsync(elasticSearchOptions.Indices.Verenigingen);
        await ElasticClient.Indices.DeleteAsync(publicElasticSearchOptions.Indices.Verenigingen);
        await ElasticClient.Indices.DeleteAsync(elasticSearchOptions.Indices.DuplicateDetection);

        await InsertNutsLauInfo();

        AdminProjectionHost = await AlbaHost.For<Admin.ProjectionHost.Program>(
            ConfigureForTesting<Admin.ProjectionHost.Program>("adminproj")
        );

        Logger = AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

        PublicProjectionHost = await AlbaHost.For<ProjectionHostProgram>(
            ConfigureForTesting<ProjectionHostProgram>("publicproj")
        );

        PublicApiHost = await AlbaHost.For<Public.Api.Program>(ConfigureForTesting<Public.Api.Program>("publicapi"));

        AcmApiHost = (
            await AlbaHost.For<Acm.Api.Program>(ConfigureForTesting<Acm.Api.Program>("acmapi"))
        ).EnsureEachCallIsAuthenticatedForAcmApi();

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
        AdminApiConfiguration = BuildConfiguration("adminapi");
    }

    public IAmazonSQS AmazonSqs { get; set; }
    public IMessageBus MessageBus { get; set; }

    private Action<IWebHostBuilder> ConfigureForTesting<TProgram>(string name)
    {
        var configuration = BuildConfiguration(name);

        return b =>
        {
            b.UseEnvironment("Development");
            b.UseContentRoot(Directory.GetCurrentDirectory());

            b.UseConfiguration(configuration);

            b.ConfigureServices(
                    (context, services) =>
                    {
                        context.HostingEnvironment.EnvironmentName = "Development";

                        services.CritterStackDefaults(options =>
                        {
                            options.ApplicationAssembly = typeof(TProgram).Assembly;

                            options.Development.GeneratedCodeMode = TypeLoadMode.Static;
                            options.Development.AssertAllPreGeneratedTypesExist = true;
                            options.Development.SourceCodeWritingEnabled = false;

                            options.Production.GeneratedCodeMode = TypeLoadMode.Static;
                            options.Production.AssertAllPreGeneratedTypesExist = true;
                            options.Production.SourceCodeWritingEnabled = false;
                        });
                    }
                )
                .UseSetting(key: "ASPNETCORE_ENVIRONMENT", value: "Development")
                .UseSetting(key: "ApplyAllDatabaseChangesDisabled", value: "true");
        };
    }

    private void ResetDatabaseBeforeHostStarts(string name)
    {
        if (name != "adminapi")
            return;

        var configuration = BuildConfiguration("adminapi");
        var databaseName = configuration["PostgreSQLOptions:database"] ?? configuration["PostgreSQLOptions:Database"];

        try
        {
            if (DatabaseTemplateHelper.IsGoldenMasterTemplateAvailable(configuration, NullLogger.Instance))
            {
                DatabaseTemplateHelper.CreateDatabaseFromTemplate(configuration, databaseName!, NullLogger.Instance);

                Console.WriteLine($"Database '{databaseName}' created successfully from template.");
                return;
            }

            using var connection = new NpgsqlConnection(GetConnectionString(configuration, "postgres"));
            using var cmd = connection.CreateCommand();

            connection.Open();
            Console.WriteLine("Golden master template not available. Recreating empty database.");
            cmd.CommandText =
                $"DROP DATABASE IF EXISTS \"{databaseName}\" WITH (FORCE);" + $"CREATE DATABASE \"{databaseName}\"";
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Database '{databaseName}' created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resetting database: {ex.Message}");

            throw;
        }
    }

    private static async Task EnsureLocalS3BucketExists()
            {
        var configuration = BuildConfiguration("publicapi");
        var bucketName = configuration["Publiq:BucketName"];

        if (string.IsNullOrWhiteSpace(bucketName))
            return;

        using var s3Client = new AmazonS3Client(
            new BasicAWSCredentials(accessKey: "dummy", secretKey: "dummy"),
            new AmazonS3Config
                {
                ServiceURL = "http://127.0.0.1:4566",
                ForcePathStyle = true,
                UseHttp = true,
            }
                    );

        try
        {
            await s3Client.PutBucketAsync(new PutBucketRequest { BucketName = bucketName });
                }
        catch (AmazonS3Exception ex) when (ex.ErrorCode is "BucketAlreadyOwnedByYou" or "BucketAlreadyExists") { }
    }

    private static IConfigurationRoot BuildConfiguration(string name) =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.development.json", optional: false)
            .AddJsonFile("appsettings.e2e.json", optional: false)
            .AddJsonFile($"appsettings.e2e.{name}.json", optional: false)
            .Build();

    private void ApplyMartenSchemaChanges()
                {
        ApplyAdminApiMartenSchemaChanges(BuildConfiguration("adminapi"));
        ApplyAdminProjectionHostMartenSchemaChanges(BuildConfiguration("adminproj"));
        ApplyPublicApiMartenSchemaChanges(BuildConfiguration("publicapi"));
        ApplyPublicProjectionHostMartenSchemaChanges(BuildConfiguration("publicproj"));
        ApplyAcmApiMartenSchemaChanges(BuildConfiguration("acmapi"));
                }

    private static void ApplyAdminApiMartenSchemaChanges(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection = configuration.GetPostgreSqlOptionsSection();
        DocumentStore? builtStore = null;

        using var documentStore = DocumentStore.For(options =>
        {
            Admin.Api.Infrastructure.MartenSetup.MartenExtensions.ConfigureStoreOptions(
                options,
                postgreSqlOptionsSection,
                isDevelopment: true,
                NullLogger<SecureMartenLogger>.Instance,
                () =>
                {
                    builtStore ??= new DocumentStore(options);

                    return builtStore.QuerySession();
                },
                AutoCreate.All
            );
        });

        try
        {
            documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
            }
        finally
            {
            builtStore?.Dispose();
            }
        }

    private static void ApplyAdminProjectionHostMartenSchemaChanges(IConfigurationRoot configuration)
        {
        var postgreSqlOptionsSection = configuration.GetPostgreSqlOptionsSection();
        var elasticSearchOptionsSection = configuration.GetElasticSearchOptionsSection();
        var elasticClient = Admin.ProjectionHost.Infrastructure.Extensions.ElasticSearchExtensions.CreateElasticClient(
            elasticSearchOptionsSection,
            NullLogger.Instance
        );

        using var documentStore = DocumentStore.For(options =>
        {
            Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder.ConfigureMartenExtensions.ConfigureStoreOptions(
                options,
                NullLogger<LocatiesGekoppeldMetGrarProjection>.Instance,
                NullLogger<LocatieZonderAdresMatchProjection>.Instance,
                elasticClient,
                isDevelopment: true,
                NullLogger<BeheerZoekenEventsConsumer>.Instance,
                NullLogger<DuplicateDetectionEventsConsumer>.Instance,
                () => NullLogger<MartenSubscription>.Instance,
                NullLogger<SecureMartenLogger>.Instance,
                postgreSqlOptionsSection,
                elasticSearchOptionsSection
            );
        });

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync(AutoCreate.All).GetAwaiter().GetResult();
        }

    private static void ApplyPublicApiMartenSchemaChanges(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection =
            Public.Api.Infrastructure.Extensions.ConfigurationExtensions.GetPostgreSqlOptionsSection(configuration);

        using var documentStore = DocumentStore.For(options =>
        {
            Public.Api.Infrastructure.Extensions.MartenExtensions.ConfigureStoreOptions(
                options,
                postgreSqlOptionsSection,
                NullLogger<SecureMartenLogger>.Instance,
                AutoCreate.All
            );
        });

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
    }

    private static void ApplyPublicProjectionHostMartenSchemaChanges(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection = configuration.GetPostgreSqlOptionsSection();
        var elasticSearchOptionsSection = configuration.GetElasticSearchOptionsSection();
        var elasticClient =
            Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder.ConfigureElasticSearchExtensions.CreateElasticClient(
                elasticSearchOptionsSection
            );

        using var documentStore = DocumentStore.For(options =>
        {
            Public.ProjectionHost.Infrastructure.Program.WebApplicationBuilder.ConfigureMartenExtensions.ConfigureStoreOptions(
                options,
                elasticClient,
                NullLogger<Public.ProjectionHost.Projections.Search.PubliekZoekenEventsConsumer>.Instance,
                NullLogger<MartenSubscription>.Instance,
                NullLogger<SecureMartenLogger>.Instance,
                postgreSqlOptionsSection,
                isDevelopment: true,
                elasticSearchOptionsSection,
                AutoCreate.All
            );
        });

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();
    }

    private static void ApplyAcmApiMartenSchemaChanges(IConfigurationRoot configuration)
    {
        var postgreSqlOptionsSection = configuration.GetPostgreSqlOptionsSection();

        using var documentStore = DocumentStore.For(options =>
        {
            Acm.Api.Infrastructure.Extensions.MartenExtensions.ConfigureStoreOptions(
                options,
                postgreSqlOptionsSection,
                NullLogger<SecureMartenLogger>.Instance,
                isDevelopment: true
            );
        });

        documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync(AutoCreate.All).GetAwaiter().GetResult();
    }

    private static string GetConnectionString(IConfiguration configuration, string database) =>
        $"host={configuration["PostgreSQLOptions:host"]};"
        + $"database={database};"
        + $"password={configuration["PostgreSQLOptions:password"]};"
        + $"username={configuration["PostgreSQLOptions:username"]}";

    public IConfigurationRoot AdminApiConfiguration { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        await StopIfCreated(AdminApiHost);
        await StopIfCreated(PublicApiHost);
        await StopIfCreated(AdminProjectionHost);
        await StopIfCreated(PublicProjectionHost);
        await StopIfCreated(AcmApiHost);

        await DisposeIfCreatedAsync(AdminApiHost);
        await DisposeIfCreatedAsync(PublicApiHost);
        await DisposeIfCreatedAsync(PublicProjectionHost);
        await DisposeIfCreatedAsync(AdminProjectionHost);
        await DisposeIfCreatedAsync(AcmApiHost);

        DisposeClients();
    }

    private static async ValueTask StopIfCreated(IAlbaHost? host)
    {
        if (host is null)
            return;

        try
        {
            await host.StopAsync();
        }
        catch (ObjectDisposedException) { }
    }

    private static async ValueTask DisposeIfCreatedAsync(IAsyncDisposable? disposable)
    {
        if (disposable is null)
            return;

        try
        {
            await disposable.DisposeAsync();
        }
        catch (ObjectDisposedException) { }
    }

    public async Task<long> ExecuteGiven(IScenario scenario)
    {
        var documentStore = AdminApiHost.DocumentStore();
        await using var session = documentStore.LightweightSession();

        session.SetHeader(MetadataHeaderNames.Initiator, value: "metadata.Initiator");
        session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
        session.CorrelationId = Guid.NewGuid().ToString();

        var givenEvents = await scenario.GivenEvents(AdminApiHost.Services.GetRequiredService<IVCodeService>());

        if (givenEvents.Length == 0)
            return 0;

        var eventConflictResolver = new EventConflictResolver(
            Array.Empty<IEventPreConflictResolutionStrategy>(),
            Array.Empty<IEventPostConflictResolutionStrategy>()
        );

        var eventStore = new EventStore(
            session,
            eventConflictResolver,
            new PersoonsgegevensProcessor(
                new PersoonsgegevensEventTransformers(),
                new VertegenwoordigerPersoonsgegevensRepository(
                    session,
                    new VertegenwoordigerPersoonsgegevensQuery(session)
                ),
                new BankrekeningnummerPersoonsgegevensRepository(
                    session,
                    new BankrekeningnummerPersoonsgegevensQuery(session)
                ),
                NullLogger<PersoonsgegevensProcessor>.Instance
            ),
            NullLogger<EventStore>.Instance
        );

        long maxSequence = 0;

        foreach (var eventsPerStream in givenEvents)
        {
            var exists = await eventStore.Exists(VCode.Hydrate(eventsPerStream.Key));

            if (exists)
            {
                var vereniging = await eventStore.Load<VerenigingState>(VCode.Hydrate(eventsPerStream.Key), null);

                var streamAction = await eventStore.Save(
                    eventsPerStream.Key,
                    vereniging.Version,
                    new CommandMetadata("metadata.Initiator", new Instant(), Guid.NewGuid(), null),
                    CancellationToken.None,
                    eventsPerStream.Value
                );

                maxSequence = streamAction.Sequence.Value;
            }
            else
            {
                var streamAction = await eventStore.SaveNew(
                    eventsPerStream.Key,
                    new CommandMetadata("metadata.Initiator", new Instant(), Guid.NewGuid(), null),
                    CancellationToken.None,
                    eventsPerStream.Value
                );

                maxSequence = streamAction.Sequence.Value;
            }
        }

        await session.SaveChangesAsync();

        return maxSequence;
    }

    public async Task RefreshIndices() => await ElasticClient.Indices.RefreshAsync(Indices.All);

    private readonly Dictionary<string, object> _ranContexts = new();
    private IServiceProvider _serviceProvider;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        DisposeIfCreated(AdminApiHost);
        DisposeIfCreated(AcmApiHost);
        DisposeIfCreated(AdminProjectionHost);
        DisposeIfCreated(PublicProjectionHost);
        DisposeIfCreated(PublicApiHost);
        DisposeIfCreated(AdminProjectionDaemon);
        DisposeClients();
    }

    private void DisposeClients()
    {
        DisposeIfCreated(SuperAdminHttpClient);
        DisposeIfCreated(UnautenticatedClient);
        DisposeIfCreated(UnauthorizedClient);
        DisposeIfCreated(AdminHttpClient);
        DisposeIfCreated(AmazonSqs);
    }

    private static void DisposeIfCreated(IDisposable? disposable)
    {
        if (disposable is null)
            return;

        try
        {
            disposable.Dispose();
        }
        catch (ObjectDisposedException) { }
    }
}
