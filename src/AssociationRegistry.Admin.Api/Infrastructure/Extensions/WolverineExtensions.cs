namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Amazon.Runtime;
using EventStore;
using Grar.Clients;
using Grar.GrarConsumer.Messaging;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.CodeGeneration;
using Kbo;
using MessageHandling.Postgres.Dubbels;
using MessageHandling.Sqs.AddressMatch;
using Messages;
using Serilog;
using Vereniging;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;

public static class WolverineExtensions
{
    public static void AddWolverine(this WebApplicationBuilder builder)
    {
        const string wolverineSchema = "public";

        builder.Host.UseWolverine(
            (context, options) =>
            {
                Log.Logger.Information("Setting up wolverine");
                options.ApplicationAssembly = typeof(Program).Assembly;
                options.Discovery.IncludeAssembly(typeof(Vereniging).Assembly);
                options.Discovery.IncludeType<TeAdresMatchenLocatieMessage>();
                options.Discovery.IncludeType<TeAdresMatchenLocatieMessageHandler>();
                options.Discovery.IncludeType<OverkoepelendeGrarConsumerMessage>();
                options.Discovery.IncludeType<OverkoepelendeGrarConsumerMessageHandler>();

                options.OnException<UnexpectedAggregateVersionDuringSyncException>().RetryWithCooldown(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5)
                );

                var grarOptions = context.Configuration.GetGrarOptions();

                if (grarOptions.Wolverine.OptimizeArtifactWorkflow)
                    options.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;

                var transportConfiguration = options.UseAmazonSqsTransport(config =>
                {
                    Log.Logger.Information(messageTemplate: "Wolverine SQS configuration: {@Config}", config);
                    config.ServiceURL = grarOptions.Wolverine.TransportServiceUrl;
                });

                if (grarOptions.Sqs.UseLocalStack)
                    transportConfiguration.Credentials(new BasicAWSCredentials(accessKey: "dummy", secretKey: "dummy"));

                ConfigureSqsQueues(options, grarOptions, context.Configuration.GetAppSettings());

                ConfigurePostgresQueues(options, context, wolverineSchema);

                if (grarOptions.Wolverine.AutoProvision)
                    transportConfiguration.AutoProvision();

                options.LogMessageStarting(LogLevel.Trace);

                Log.Logger.Information(messageTemplate: "Wolverine Transport SQS configuration: {@TransportConfig}",
                                       transportConfiguration);
            });
    }

    private static void ConfigureSqsQueues(WolverineOptions options, GrarOptions grarOptions, AppSettings appSettings)
    {
        ConfigureAddressMatchPublisher(options, grarOptions.Sqs.AddressMatchQueueName);

        ConfigureAddressMatchListener(options, grarOptions.Sqs.AddressMatchQueueName,
                                      grarOptions.Sqs.AddressMatchDeadLetterQueueName);

        ConfigureGrarSyncListener(options, grarOptions.Sqs.GrarSyncQueueName, grarOptions.Sqs.GrarSyncDeadLetterQueueName,
                                  grarOptions.Sqs.GrarSyncQueueListenerEnabled);

        ConfigureKboSyncPublisher(options, appSettings);
    }

    private static void ConfigureKboSyncPublisher(WolverineOptions options, AppSettings appSettings)
    {
        options.PublishMessage<TeSynchroniserenKboNummerMessage>()
               .ToSqsQueue(appSettings.KboSyncQueueName)
               .SendRawJsonMessage()
               .MessageBatchSize(1);
    }

    private static void ConfigurePostgresQueues(
        WolverineOptions options,
        HostBuilderContext context,
        string wolverineSchema)
    {
        const string AanvaardDubbeleVerenigingQueueName = "aanvaard-dubbele-vereniging-queue";

        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessage>();
        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessageHandler>();

        var connectionString = context.Configuration.GetPostgreSqlOptionsSection().GetConnectionString();

        options.UsePostgresqlPersistenceAndTransport(connectionString, wolverineSchema, wolverineSchema);

        options.PublishMessage<AanvaardDubbeleVerenigingMessage>()
               .ToPostgresqlQueue(AanvaardDubbeleVerenigingQueueName);
        options.ListenToPostgresqlQueue(AanvaardDubbeleVerenigingQueueName);
    }

    private static void ConfigureAddressMatchPublisher(WolverineOptions options, string sqsQueueName)
    {
        options.PublishMessage<TeAdresMatchenLocatieMessage>()
               .ToSqsQueue(sqsQueueName)
               .MessageBatchSize(1);
    }

    private static void ConfigureAddressMatchListener(WolverineOptions options, string sqsQueueName, string sqsDeadLetterQueueName)
    {
        options.ListenToSqsQueue(sqsQueueName, configure: configure =>
                {
                    configure.MaxNumberOfMessages = 1;
                    configure.DeadLetterQueueName = sqsDeadLetterQueueName;
                })
               .ConfigureDeadLetterQueue(sqsDeadLetterQueueName)
               .MaximumParallelMessages(1);
    }

    private static void ConfigureGrarSyncListener(
        WolverineOptions options,
        string sqsQueueName,
        string sqsDeadLetterQueueName,
        bool enabled)
    {
        if (!enabled)
        {
            Log.Logger.Information("Not setting up GRAR Sync Listener.");

            return;
        }

        Log.Logger.Information(messageTemplate: "Setting up GRAR Sync Listener for queue '{Queue}' with dlq '{Dlq}'.",
                               sqsQueueName,
                               sqsDeadLetterQueueName
        );

        options.PublishMessage<OverkoepelendeGrarConsumerMessage>()
               .ToSqsQueue(sqsQueueName);

         options.ListenToSqsQueue(sqsQueueName, configure: configure =>
                {
                    configure.MaxNumberOfMessages = 1;
                    configure.DeadLetterQueueName = sqsDeadLetterQueueName;
                })
               .ConfigureDeadLetterQueue(sqsDeadLetterQueueName, configure: queue =>
                {
                    queue.DeadLetterQueueName = sqsDeadLetterQueueName;
                })
               .MaximumParallelMessages(1);
    }
}
