namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using AssociationRegistry.CommandHandling.MagdaSync.SyncKbo;
using AssociationRegistry.CommandHandling.MagdaSync.SyncKsz;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.EventStore.ConflictResolution;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Integrations.Magda;
using AssociationRegistry.Integrations.Magda.CallReferences;
using AssociationRegistry.Integrations.Magda.Onderneming;
using AssociationRegistry.Integrations.Magda.Persoon;
using AssociationRegistry.Integrations.Magda.Persoon.Validation;
using AssociationRegistry.Integrations.Magda.Shared.Models;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Magda.Persoon;
using AssociationRegistry.Integrations.Slack;
using AssociationRegistry.KboMutations.Configuration;
using AssociationRegistry.KboMutations.Notifications;
using AssociationRegistry.KboMutations.SyncLambda.JsonSerialization;
using AssociationRegistry.KboMutations.SyncLambda.Logging;
using AssociationRegistry.KboMutations.SyncLambda.Telemetry;
using AssociationRegistry.MartenDb.Store;
using AssociationRegistry.MartenDb.Transformers;
using AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;
using AssociationRegistry.Persoonsgegevens;
using Configuration;
using JasperFx;
using JasperFx.Events;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using Wolverine;
using global::Wolverine.Marten;
using Wolverine.Postgresql;
using PostgreSqlOptionsSection = Configuration.PostgreSqlOptionsSection;
using IEventStore = AssociationRegistry.MartenDb.Store.IEventStore;
using EventStore = AssociationRegistry.MartenDb.Store.EventStore;

public static class HostConfiguration
{
    public static async Task<IHost> CreateHost(ILambdaContext context, IConfigurationRoot configuration)
    {
        var telemetryManager = new TelemetryManager(context.Logger, configuration);
        var paramNamesConfiguration = GetParamNamesConfiguration(configuration);
        var ssmClientWrapper = new SsmClientWrapper(new AmazonSimpleSystemsManagementClient());

        // Get connection string and Magda options first
        var magdaOptions = await GetMagdaOptionsAsync(ssmClientWrapper, paramNamesConfiguration, configuration);
        var connectionString = await BuildConnectionStringAsync(ssmClientWrapper, paramNamesConfiguration, configuration);

        var host = await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Add configuration
                services.AddSingleton(configuration);
                services.AddSingleton(paramNamesConfiguration);
                services.AddSingleton(telemetryManager);
                services.AddSingleton<ILambdaLogger>(context.Logger);
                services.AddSingleton(magdaOptions);

                var kboSyncConfiguration = configuration.GetSection(KboSyncConfiguration.Section)
                                                       .Get<KboSyncConfiguration>()
                                        ?? throw new InvalidOperationException("Could not load KboSyncConfiguration");
                services.AddSingleton(kboSyncConfiguration);

                // Add Magda services
                services.AddSingleton<IMagdaCallReferenceRepository, MagdaCallReferenceRepository>();
                services.AddSingleton<IMagdaCallReferenceService, MagdaCallReferenceService>();
                services.AddSingleton<IMagdaRegistreerInschrijvingValidator, MagdaRegistreerInschrijvingValidator>();
                services.AddSingleton<IMagdaGeefPersoonValidator, MagdaGeefPersoonValidator>();
                services.AddSingleton<IMagdaClient, MagdaClient>();
                services.AddSingleton<IVertegenwoordigerPersoonsgegevensQuery, VertegenwoordigerPersoonsgegevensQuery>();
                services.AddSingleton<PersoonsgegevensEventTransformers>();
                services.AddSingleton<IEventPreConflictResolutionStrategy, EmptyConflictResolutionStrategy>();
                services.AddSingleton<IEventPostConflictResolutionStrategy, EmptyConflictResolutionStrategy>();
                services.AddSingleton<IMagdaRegistreerInschrijvingService, MagdaRegistreerInschrijvingService>();
                services.AddSingleton<IMagdaGeefPersoonService, MagdaGeefPersoonService>();
                services.AddSingleton<IMagdaSyncGeefVerenigingService, SyncGeefVerenigingService>();

                // Add repository
                services.AddSingleton<IVerenigingsRepository, VerenigingsRepository>();
                services.AddSingleton<IEventStore, EventStore>();
                services.AddSingleton(sp =>
                    new EventConflictResolver(
                        Array.Empty<IEventPreConflictResolutionStrategy>(),
                        Array.Empty<IEventPostConflictResolutionStrategy>()));
                services.AddSingleton<IPersoonsgegevensProcessor, PersoonsgegevensProcessor>();
                services.AddSingleton<IVertegenwoordigerPersoonsgegevensRepository, VertegenwoordigerPersoonsgegevensRepository>();

                // Add handlers
                services.AddSingleton<SyncKboCommandHandler>();
                services.AddSingleton<SyncKszMessageHandler>();
                services.AddSingleton<MessageProcessor>();
            })
            .ConfigureServices(async services =>
            {
                // Add notifier (needs async)
                var notifier = await CreateNotifierAsync(ssmClientWrapper, paramNamesConfiguration, context.Logger);
                services.AddSingleton(notifier);
            })
            .ConfigureLogging(builder =>
            {
                builder.AddProvider(new LambdaLoggerProvider(context.Logger));
                telemetryManager.ConfigureLogging(builder);
            })
            .UseWolverine(opts =>
            {
                opts.Services.AddMarten(sp =>
                {
                    var storeOptions = ConfigureStoreOptions(connectionString);
                    return storeOptions;
                })
                .IntegrateWithWolverine();

                opts.Discovery.IncludeType<MarkeerVertegenwoordigerAlsOverledenMessage>();

                const string naam = "markeer-v-als-overleden-queue";
                opts.PublishMessage<MarkeerVertegenwoordigerAlsOverledenMessage>()
                    .ToPostgresqlQueue(naam);

                opts.PersistMessagesWithPostgresql(connectionString, "public")
                    .EnableMessageTransport();

                opts.AutoBuildMessageStorageOnStartup = AutoCreate.All;

                opts.Policies.AutoApplyTransactions();
            })
            .StartAsync();

        return host;
    }

    private static ParamNamesConfiguration GetParamNamesConfiguration(IConfigurationRoot configuration)
    {
        return configuration
              .GetSection(ParamNamesConfiguration.Section)
              .Get<ParamNamesConfiguration>()
            ?? throw new InvalidOperationException("Could not load ParamNamesConfiguration");
    }

    private static async Task<MagdaOptionsSection> GetMagdaOptionsAsync(
        SsmClientWrapper ssmClient,
        ParamNamesConfiguration paramNamesConfiguration,
        IConfigurationRoot configuration)
    {
        var magdaOptions = configuration.GetSection(MagdaOptionsSection.SectionName)
                                        .Get<MagdaOptionsSection>()
                        ?? throw new ArgumentException("Could not load MagdaOptions");


        // if (!string.IsNullOrEmpty(paramNamesConfiguration.MagdaCertificate))
        // {
        //     magdaOptions.ClientCertificate = await ssmClient.GetParameterAsync(paramNamesConfiguration.MagdaCertificate);
        //     magdaOptions.ClientCertificatePassword = await ssmClient.GetParameterAsync(paramNamesConfiguration.MagdaCertificatePassword);
        // }

        return magdaOptions;
    }

    private static async Task<string> BuildConnectionStringAsync(
        SsmClientWrapper ssmClientWrapper,
        ParamNamesConfiguration paramNames,
        IConfigurationRoot configuration)
    {
        var postgresSection = configuration.GetSection(PostgreSqlOptionsSection.SectionName)
                                            .Get<PostgreSqlOptionsSection>()
                           ?? throw new ApplicationException("PostgresSqlOptions section not found");

        if (!postgresSection.IsComplete)
            throw new ApplicationException("PostgresSqlOptions is missing some values");

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

    private static async Task<INotifier> CreateNotifierAsync(
        SsmClientWrapper ssmClientWrapper,
        ParamNamesConfiguration paramNamesConfiguration,
        ILambdaLogger logger)
    {
        return await new NotifierFactory(ssmClientWrapper, paramNamesConfiguration, logger).Create();
    }
}
