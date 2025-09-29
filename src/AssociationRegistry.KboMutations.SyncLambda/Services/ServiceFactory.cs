namespace AssociationRegistry.KboMutations.SyncLambda.Services;

using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using AssociationRegistry.EventStore;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.KboMutations.Configuration;
using AssociationRegistry.KboMutations.Notifications;
using AssociationRegistry.KboMutations.SyncLambda.JsonSerialization;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.Models;
using Configuration;
using EventStore.ConflictResolution;
using Integrations.Slack;
using JasperFx;
using JasperFx.Events;
using Logging;
using Marten;
using Marten.Events;
using Marten.Services;
using MartenDb.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using Telemetry;
using Weasel.Core;
using PostgreSqlOptionsSection = Configuration.PostgreSqlOptionsSection;

public class ServiceFactory
{
    private readonly IConfigurationRoot _configuration;
    private readonly ILambdaLogger _logger;
    private readonly TelemetryManager _telemetryManager;

    public ServiceFactory(IConfigurationRoot configuration, ILambdaLogger logger, TelemetryManager telemetryManager)
    {
        _configuration = configuration;
        _logger = logger;
        _telemetryManager = telemetryManager;
    }

    public async Task<LambdaServices> CreateServicesAsync()
    {
        var paramNamesConfiguration = GetParamNamesConfiguration();
        var ssmClientWrapper = new SsmClientWrapper(new AmazonSimpleSystemsManagementClient());

        var messageProcessor = CreateMessageProcessor();
        var loggerFactory = CreateLoggerFactory();
        var logger = loggerFactory.CreateLogger<ServiceFactory>();
        var magdaOptions = await GetMagdaOptionsAsync(ssmClientWrapper, paramNamesConfiguration, logger);
        var store = await SetUpDocumentStoreAsync(ssmClientWrapper, paramNamesConfiguration, logger);
        var repository = CreateRepository(store, loggerFactory);
        var geefOndernemingService = CreateGeefOndernemingService(store, magdaOptions, loggerFactory);
        var registreerInschrijvingService = CreateRegistreerInschrijvingService(store, magdaOptions, loggerFactory);
        var notifier = await CreateNotifierAsync(ssmClientWrapper, paramNamesConfiguration);

        return new LambdaServices(
            messageProcessor,
            loggerFactory,
            registreerInschrijvingService,
            geefOndernemingService,
            repository,
            notifier
        );
    }

    private ParamNamesConfiguration GetParamNamesConfiguration()
    {
        return _configuration
              .GetSection(ParamNamesConfiguration.Section)
              .Get<ParamNamesConfiguration>()
            ?? throw new InvalidOperationException("Could not load ParamNamesConfiguration");
    }

    private MessageProcessor CreateMessageProcessor()
    {
        var awsConfigurationSection = _configuration.GetSection("KboSync");

        return new MessageProcessor(new KboSyncConfiguration
        {
            MutationFileQueueUrl = awsConfigurationSection[nameof(WellKnownQueueNames.MutationFileQueueUrl)],
            SyncQueueUrl = awsConfigurationSection[nameof(WellKnownQueueNames.SyncQueueUrl)]!
        });
    }

    private ILoggerFactory CreateLoggerFactory()
    {
        return LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new LambdaLoggerProvider(_logger));
            _telemetryManager.ConfigureLogging(builder);
        });
    }

    private async Task<MagdaOptionsSection> GetMagdaOptionsAsync(
        SsmClientWrapper ssmClient,
        ParamNamesConfiguration paramNamesConfiguration,
        ILogger<ServiceFactory> logger)
    {
        var magdaOptions = _configuration.GetSection(MagdaOptionsSection.SectionName)
                                         .Get<MagdaOptionsSection>()
                        ?? throw new ArgumentException("Could not load MagdaOptions");

        if (string.IsNullOrEmpty(paramNamesConfiguration.MagdaCertificate))
        {
            logger.LogInformation("Magda certificate parameter name is not set, skipping certificate retrieval.");
            return magdaOptions;
        }

        magdaOptions.ClientCertificate = await ssmClient.GetParameterAsync(paramNamesConfiguration.MagdaCertificate);
        magdaOptions.ClientCertificatePassword = await ssmClient.GetParameterAsync(paramNamesConfiguration.MagdaCertificatePassword);

        return magdaOptions;
    }

    private async Task<DocumentStore> SetUpDocumentStoreAsync(
        SsmClientWrapper ssmClientWrapper,
        ParamNamesConfiguration paramNames,
        ILogger<ServiceFactory> logger)
    {
        var postgresSection = _configuration.GetSection(PostgreSqlOptionsSection.SectionName)
                                            .Get<PostgreSqlOptionsSection>()
                           ?? throw new ApplicationException("PostgresSqlOptions section not found");

        if (!postgresSection.IsComplete)
            throw new ApplicationException("PostgresSqlOptions is missing some values");

        logger.LogInformation("Using PostgreSQL options: {Host}, {Database}",
            postgresSection.Host, postgresSection.Database);

        var connectionString = await BuildConnectionStringAsync(postgresSection, ssmClientWrapper, paramNames);
        var opts = ConfigureStoreOptions(connectionString);

        return new DocumentStore(opts);
    }

    private async Task<string> BuildConnectionStringAsync(PostgreSqlOptionsSection postgresSection, SsmClientWrapper ssmClientWrapper, ParamNamesConfiguration paramNames)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = postgresSection.Host,
            Database = postgresSection.Database,
            Username = postgresSection.Username,
            Port = 5432,
            Password = await ssmClientWrapper.GetParameterAsync(paramNames.PostgresPassword)
        };

        return connectionStringBuilder.ToString();
    }

    private static StoreOptions ConfigureStoreOptions(string connectionString)
    {
        var opts = new StoreOptions();
        opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);
        opts.Connection(connectionString);
        opts.Events.StreamIdentity = StreamIdentity.AsString;
        opts.UseNewtonsoftForSerialization(configure: settings =>
        {
            settings.DateParseHandling = DateParseHandling.None;
            settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
            settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
        });
        opts.Events.MetadataConfig.EnableAll();
        opts.AutoCreateSchemaObjects = AutoCreate.None;

        var eventTypes = typeof(AssociationRegistry.Events.IEvent).Assembly
                                                                  .GetTypes()
                                                                  .Where(t => typeof(AssociationRegistry.Events.IEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                                                                  .ToList();

        opts.Events.AddEventTypes(eventTypes);

        return opts;
    }

    private static VerenigingsRepository CreateRepository(DocumentStore store, ILoggerFactory loggerFactory)
    {
        var eventConflictResolver = new EventConflictResolver(
            Array.Empty<IEventPreConflictResolutionStrategy>(),
            Array.Empty<IEventPostConflictResolutionStrategy>());

        return new VerenigingsRepository(
            new EventStore(store, eventConflictResolver, loggerFactory.CreateLogger<EventStore>()));
    }

    private static MagdaGeefVerenigingService CreateGeefOndernemingService(DocumentStore store, MagdaOptionsSection magdaOptions, ILoggerFactory loggerFactory)
    {
        return new MagdaGeefVerenigingService(
            new MagdaCallReferenceRepository(store.LightweightSession()),
            new MagdaClient(magdaOptions, loggerFactory.CreateLogger<MagdaClient>()),
            loggerFactory.CreateLogger<MagdaGeefVerenigingService>());
    }

    private static MagdaRegistreerInschrijvingService CreateRegistreerInschrijvingService(DocumentStore store, MagdaOptionsSection magdaOptions, ILoggerFactory loggerFactory)
    {
        return new MagdaRegistreerInschrijvingService(
            new MagdaCallReferenceRepository(store.LightweightSession()),
            new MagdaClient(magdaOptions, loggerFactory.CreateLogger<MagdaClient>()),
            loggerFactory.CreateLogger<MagdaRegistreerInschrijvingService>());
    }

    private async Task<INotifier> CreateNotifierAsync(SsmClientWrapper ssmClientWrapper, ParamNamesConfiguration paramNamesConfiguration)
    {
        return await new NotifierFactory(ssmClientWrapper, paramNamesConfiguration, _logger).Create();
    }
}
