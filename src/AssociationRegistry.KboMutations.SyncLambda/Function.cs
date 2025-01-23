using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Amazon.SimpleSystemsManagement;
using AssociationRegistry.EventStore;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.KboMutations.SyncLambda.Configuration;
using AssociationRegistry.KboMutations.SyncLambda.JsonSerialization;
using AssociationRegistry.KboMutations.SyncLambda.Logging;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Models;
using Marten;
using Marten.Events;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using Weasel.Core;
using PostgreSqlOptionsSection = AssociationRegistry.KboMutations.SyncLambda.Logging.PostgreSqlOptionsSection;
using SsmClientWrapper = AssociationRegistry.KboMutations.SsmClientWrapper;

namespace AssociationRegistry.KboMutations.SyncLambda;

using KboMutations.Configuration;
using Notifications;

public class Function
{
    private static async Task Main()
    {
        var handler = FunctionHandler;
        await LambdaBootstrapBuilder.Create(handler, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

    private static async Task FunctionHandler(SQSEvent @event, ILambdaContext context)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables();

        var configuration = configurationBuilder.Build();
        var awsConfigurationSection = configuration
            .GetSection("KboSync");

        var paramNamesConfiguration = configuration
            .GetSection(ParamNamesConfiguration.Section)
            .Get<ParamNamesConfiguration>();

        var processor = new MessageProcessor(new KboSyncConfiguration
        {
            MutationFileQueueUrl = awsConfigurationSection[nameof(WellKnownQueueNames.MutationFileQueueUrl)],
            SyncQueueUrl = awsConfigurationSection[nameof(WellKnownQueueNames.SyncQueueUrl)]!
        });

        var ssmClientWrapper = new SsmClientWrapper(new AmazonSimpleSystemsManagementClient());
        var magdaOptions = await GetMagdaOptions(configuration, ssmClientWrapper, paramNamesConfiguration);

        var store = await SetUpDocumentStore(configuration, context.Logger, ssmClientWrapper, paramNamesConfiguration);

        var eventConflictResolver = new EventConflictResolver(Array.Empty<IEventPreConflictResolutionStrategy>(),
            Array.Empty<IEventPostConflictResolutionStrategy>());

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new LambdaLoggerProvider(context.Logger));
        });

        var repository = new VerenigingsRepository(new EventStore.EventStore(store, eventConflictResolver, loggerFactory.CreateLogger<EventStore.EventStore>()));

        var geefOndernemingService = new MagdaGeefVerenigingService(
            new MagdaCallReferenceRepository(store.LightweightSession()),
            new MagdaClient(magdaOptions, loggerFactory.CreateLogger<MagdaClient>()),
            new TemporaryMagdaVertegenwoordigersSection(),
            loggerFactory.CreateLogger<MagdaGeefVerenigingService>());

        var registreerinschrijvingService = new MagdaRegistreerInschrijvingService(
            new MagdaCallReferenceRepository(store.LightweightSession()),
            new MagdaClient(magdaOptions, loggerFactory.CreateLogger<MagdaClient>()),
            loggerFactory.CreateLogger<MagdaRegistreerInschrijvingService>());

        var notifier = await new NotifierFactory(ssmClientWrapper, paramNamesConfiguration, context.Logger)
            .Create();

        context.Logger.LogInformation($"{@event.Records.Count} RECORDS RECEIVED INSIDE SQS EVENT");
        await processor!.ProcessMessage(
            @event,
            loggerFactory,
            registreerinschrijvingService,
            geefOndernemingService,
            repository,
            notifier,
            CancellationToken.None);
        context.Logger.LogInformation($"{@event.Records.Count} RECORDS PROCESSED BY THE MESSAGE PROCESSOR");
    }

    private static async Task<MagdaOptionsSection> GetMagdaOptions(IConfiguration config,
        SsmClientWrapper ssmClient,
        ParamNamesConfiguration? paramNamesConfiguration)
    {
        var magdaOptions = config.GetSection(MagdaOptionsSection.SectionName)
            .Get<MagdaOptionsSection>();

        if (magdaOptions is null)
            throw new ArgumentException("Could not load MagdaOptions");

        magdaOptions.ClientCertificate = await ssmClient.GetParameterAsync(paramNamesConfiguration.MagdaCertificate);
        magdaOptions.ClientCertificatePassword =
            await ssmClient.GetParameterAsync(paramNamesConfiguration.MagdaCertificatePassword);
        return magdaOptions;
    }

    private static async Task<DocumentStore> SetUpDocumentStore(IConfiguration config,
        ILambdaLogger contextLogger,
        SsmClientWrapper ssmClientWrapper,
        ParamNamesConfiguration paramNames)
    {
        var postgresSection =
            config.GetSection(PostgreSqlOptionsSection.SectionName)
                .Get<PostgreSqlOptionsSection>();

        if (!postgresSection.IsComplete)
            throw new ApplicationException("PostgresSqlOptions is missing some values");

        var opts = new StoreOptions();
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder();
        connectionStringBuilder.Host = postgresSection.Host;
        connectionStringBuilder.Database = postgresSection.Database;
        connectionStringBuilder.Username = postgresSection.Username;
        connectionStringBuilder.Port = 5432;
        connectionStringBuilder.Password = await ssmClientWrapper.GetParameterAsync(paramNames.PostgresPassword);
        opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);

        var connectionString = connectionStringBuilder.ToString();

        opts.Connection(connectionString);
        opts.Events.StreamIdentity = StreamIdentity.AsString;
        opts.Serializer(CreateCustomMartenSerializer());
        opts.Events.MetadataConfig.EnableAll();
        opts.AutoCreateSchemaObjects = AutoCreate.None;

        opts.Events.AddEventTypes( typeof(AssociationRegistry.Events.IEvent).Assembly
                                                                            .GetTypes()
                                                                            .Where(t => typeof(AssociationRegistry.Events.IEvent)
                                                                                      .IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                                                                            .ToList());

        var store = new DocumentStore(opts);
        return store;
    }

    public static JsonNetSerializer CreateCustomMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();

        jsonNetSerializer.Customize(
            s =>
            {
                s.DateParseHandling = DateParseHandling.None;
                s.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                s.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });

        return jsonNetSerializer;
    }
}

[JsonSerializable(typeof(SQSEvent))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
}
