namespace AssociationRegistry.KboMutations.SyncLambda.Services;

using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.KboMutations.Configuration;
using AssociationRegistry.KboMutations.Notifications;
using Configuration;
using EventStore.ConflictResolution;
using Integrations.Magda.CallReferences;
using Integrations.Magda.Onderneming;
using Integrations.Magda.Persoon.Validation;
using Integrations.Magda.Shared.Models;
using Integrations.Slack;
using JasperFx;
using JasperFx.Events;
using JsonSerialization;
using Logging;
using MagdaSync.SyncKbo;
using MagdaSync.SyncKsz;
using MagdaSync.SyncKsz.Queries;
using Marten;
using MartenDb.BankrekeningnummerPersoonsgegevens;
using MartenDb.Store;
using MartenDb.Transformers;
using MartenDb.Upcasters.Persoonsgegevens;
using MartenDb.VertegenwoordigerPersoonsgegevens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Npgsql;
using Telemetry;
using PostgreSqlOptionsSection = Configuration.PostgreSqlOptionsSection;

public class ServiceFactory
{
    private readonly IConfigurationRoot _configuration;
    private readonly ILambdaLogger _logger;
    private readonly TelemetryManager _telemetryManager;
    private DocumentStore _store;

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

        var loggerFactory = CreateLoggerFactory();
        var logger = loggerFactory.CreateLogger<ServiceFactory>();

        var magdaOptions = await GetMagdaOptionsAsync(
            ssmClient: ssmClientWrapper,
            paramNamesConfiguration: paramNamesConfiguration,
            logger: logger
        );

        _store = await SetUpDocumentStoreAsync(
            ssmClientWrapper: ssmClientWrapper,
            paramNames: paramNamesConfiguration,
            querySessionFunc: GetQuerySession,
            logger: logger
        );

        var session = _store.LightweightSession();

        var eventConflictResolver = new EventConflictResolver(
            Array.Empty<IEventPreConflictResolutionStrategy>(),
            Array.Empty<IEventPostConflictResolutionStrategy>()
        );

        var eventStore = new EventStore(
            session: session,
            conflictResolver: eventConflictResolver,
            new PersoonsgegevensProcessor(
                new PersoonsgegevensEventTransformers(),
                new VertegenwoordigerPersoonsgegevensRepository(
                    session: session,
                    new VertegenwoordigerPersoonsgegevensQuery(session)
                ),
                new BankrekeningnummerPersoonsgegevensRepository(
                    session: session,
                    new BankrekeningnummerPersoonsgegevensQuery(session)
                ),
                loggerFactory.CreateLogger<PersoonsgegevensProcessor>()
            ),
            loggerFactory.CreateLogger<EventStore>()
        );

        var aggregateSession = new AggregateSession(eventStore);
        var queryService = new VerenigingStateQueryService(session);
        var referenceRepository = new MagdaCallReferenceRepository(_store.LightweightSession());

        var magdaClient = new MagdaClient(
            magdaOptions: magdaOptions,
            new MagdaCallReferenceService(referenceRepository),
            new MagdaRegistreerInschrijvingValidator(
                loggerFactory.CreateLogger<MagdaRegistreerInschrijvingValidator>()
            ),
            new MagdaGeefPersoonValidator(loggerFactory.CreateLogger<MagdaGeefPersoonValidator>()),
            loggerFactory.CreateLogger<MagdaClient>()
        );

        var registreerInschrijvingService = CreateRegistreerInschrijvingService(
            magdaOptions: magdaOptions,
            loggerFactory: loggerFactory,
            referenceRepository: referenceRepository
        );

        var vertegenwoordigerPersoonsgegevensRepository = new VertegenwoordigerPersoonsgegevensRepository(
            session: session,
            new VertegenwoordigerPersoonsgegevensQuery(session)
        );

        var vzerVertegenwoordigerForInszQuery = new VzerVertegenwoordigerForInszQuery(
            vertegenwoordigerPersoonsgegevensRepository,
            new FilterVzerOnlyQuery(session),
            loggerFactory.CreateLogger<VzerVertegenwoordigerForInszQuery>()
        );

        var notifier = await CreateNotifierAsync(
            ssmClientWrapper: ssmClientWrapper,
            paramNamesConfiguration: paramNamesConfiguration,
            loggerFactory: loggerFactory
        );

        var kboSyncHandler = new SyncKboCommandHandler(
            registreerInschrijvingService: registreerInschrijvingService,
            new SyncGeefVerenigingService(
                magdaClient: magdaClient,
                loggerFactory.CreateLogger<SyncGeefVerenigingService>()
            ),
            notifier: notifier,
            loggerFactory.CreateLogger<SyncKboCommandHandler>(),
            metrics: _telemetryManager.Metrics
        );

        var kszSyncHandler = new SyncKszMessageHandler(
            vzerVertegenwoordigerForInszQuery: vzerVertegenwoordigerForInszQuery,
            aggregateSession: aggregateSession,
            loggerFactory.CreateLogger<SyncKszMessageHandler>()
        );

        var messageProcessor = CreateMessageProcessor(loggerFactory.CreateLogger<MessageProcessor>());

        return new LambdaServices(
            MessageProcessor: messageProcessor,
            LoggerFactory: loggerFactory,
            KboSyncHandler: kboSyncHandler,
            KszSyncHandler: kszSyncHandler,
            Repository: aggregateSession,
            QueryService: queryService,
            Notifier: notifier
        );
    }

    private IQuerySession GetQuerySession() => _store.QuerySession();

    private ParamNamesConfiguration GetParamNamesConfiguration() =>
        _configuration.GetSection(ParamNamesConfiguration.Section).Get<ParamNamesConfiguration>()
        ?? throw new InvalidOperationException("Could not load ParamNamesConfiguration");

    private MessageProcessor CreateMessageProcessor(ILogger<MessageProcessor> logger)
    {
        var awsConfigurationSection = _configuration.GetSection("KboSync");

        return new MessageProcessor(
            new KboSyncConfiguration
            {
                MutationFileQueueUrl = awsConfigurationSection[nameof(WellKnownQueueNames.MutationFileQueueUrl)],
                SyncQueueUrl = awsConfigurationSection[nameof(WellKnownQueueNames.SyncQueueUrl)]!,
            },
            logger: logger
        );
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
        ILogger<ServiceFactory> logger
    )
    {
        var magdaOptions =
            _configuration.GetSection(MagdaOptionsSection.SectionName).Get<MagdaOptionsSection>()
            ?? throw new ArgumentException("Could not load MagdaOptions");

        if (string.IsNullOrEmpty(paramNamesConfiguration.MagdaCertificate))
        {
            logger.LogInformation("Magda certificate parameter name is not set, skipping certificate retrieval.");

            return magdaOptions;
        }

        magdaOptions.ClientCertificate = await ssmClient.GetParameterAsync(paramNamesConfiguration.MagdaCertificate);

        magdaOptions.ClientCertificatePassword = await ssmClient.GetParameterAsync(
            paramNamesConfiguration.MagdaCertificatePassword
        );

        return magdaOptions;
    }

    private async Task<DocumentStore> SetUpDocumentStoreAsync(
        SsmClientWrapper ssmClientWrapper,
        ParamNamesConfiguration paramNames,
        Func<IQuerySession> querySessionFunc,
        ILogger<ServiceFactory> logger
    )
    {
        var postgresSection =
            _configuration.GetSection(PostgreSqlOptionsSection.SectionName).Get<PostgreSqlOptionsSection>()
            ?? throw new ApplicationException("PostgresSqlOptions section not found");

        if (!postgresSection.IsComplete)
            throw new ApplicationException("PostgresSqlOptions is missing some values");

        logger.LogInformation(
            message: "Using PostgreSQL options: {Host}, {Database}",
            postgresSection.Host,
            postgresSection.Database
        );

        var connectionString = await BuildConnectionStringAsync(
            postgresSection: postgresSection,
            ssmClientWrapper: ssmClientWrapper,
            paramNames: paramNames
        );

        var opts = ConfigureStoreOptions(connectionString: connectionString, querySessionFunc: querySessionFunc);

        return new DocumentStore(opts);
    }

    private async Task<string> BuildConnectionStringAsync(
        PostgreSqlOptionsSection postgresSection,
        SsmClientWrapper ssmClientWrapper,
        ParamNamesConfiguration paramNames
    )
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = postgresSection.Host,
            Database = postgresSection.Database,
            Username = postgresSection.Username,
            Port = 5432,
            Password = await ssmClientWrapper.GetParameterAsync(paramNames.PostgresPassword),
        };

        return connectionStringBuilder.ToString();
    }

    private static NpgsqlDataSource BuildDataSource(string connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

        dataSourceBuilder.ConfigureTracing(options =>
        {
            // Disable the "time-to-first-read" event to reduce noise in traces
            options.EnableFirstResponseEvent(false);
        });

        return dataSourceBuilder.Build();
    }

    private static StoreOptions ConfigureStoreOptions(string connectionString, Func<IQuerySession> querySessionFunc)
    {
        var opts = new StoreOptions();
        opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);

        var dataSource = BuildDataSource(connectionString);
        opts.Connection(dataSource);
        opts.Events.StreamIdentity = StreamIdentity.AsString;

        opts.UseNewtonsoftForSerialization(configure: settings =>
        {
            settings.DateParseHandling = DateParseHandling.None;
            settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
            settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
        });

        opts.Events.MetadataConfig.EnableAll();
        opts.AutoCreateSchemaObjects = AutoCreate.None;
        opts.UpcastEvents(querySessionFunc);

        var eventTypes = typeof(AssociationRegistry.Events.IEvent)
            .Assembly.GetTypes()
            .Where(t => typeof(AssociationRegistry.Events.IEvent).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .ToList();

        opts.Events.AddEventTypes(eventTypes);

        return opts;
    }

    private static MagdaRegistreerInschrijvingService CreateRegistreerInschrijvingService(
        MagdaOptionsSection magdaOptions,
        ILoggerFactory loggerFactory,
        MagdaCallReferenceRepository referenceRepository
    ) =>
        new(
            new MagdaClient(
                magdaOptions: magdaOptions,
                new MagdaCallReferenceService(referenceRepository),
                new MagdaRegistreerInschrijvingValidator(NullLogger<MagdaRegistreerInschrijvingValidator>.Instance),
                new MagdaGeefPersoonValidator(NullLogger<MagdaGeefPersoonValidator>.Instance),
                loggerFactory.CreateLogger<MagdaClient>()
            ),
            loggerFactory.CreateLogger<MagdaRegistreerInschrijvingService>()
        );

    private async Task<INotifier> CreateNotifierAsync(
        SsmClientWrapper ssmClientWrapper,
        ParamNamesConfiguration paramNamesConfiguration,
        ILoggerFactory loggerFactory
    ) =>
        await new NotifierFactory(
            ssmClientWrapper: ssmClientWrapper,
            paramNames: paramNamesConfiguration,
            loggerFactory: loggerFactory
        ).Create();
}
