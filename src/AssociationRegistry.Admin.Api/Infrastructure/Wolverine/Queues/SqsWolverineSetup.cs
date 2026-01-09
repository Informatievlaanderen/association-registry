namespace AssociationRegistry.Admin.Api.Infrastructure.Wolverine.Queues;

using Amazon.Runtime;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using AssociationRegistry.CommandHandling.Grar.GrarConsumer.Messaging;
using Contracts.KboSync;
using Hosts.Configuration.ConfigurationBindings;
using global::Wolverine;
using global::Wolverine.AmazonSqs;
using Hosts.Configuration;
using JasperFx.CodeGeneration;
using Serilog;

internal static class SqsWolverineSetup
{
    public static void ConfigureSqsQueues(WolverineOptions options, ConfigurationManager configuration)
    {
        var grarOptions = configuration.GetGrarOptions();

        var transportConfiguration = options.UseAmazonSqsTransport(config =>
        {
            Log.Logger.Information(messageTemplate: "Wolverine SQS configuration: {@Config}", config);
            config.ServiceURL = grarOptions.Wolverine.TransportServiceUrl;
        });

        options.UseNewtonsoftForSerialization();

        if (grarOptions.Sqs.UseLocalStack)
            transportConfiguration.Credentials(new BasicAWSCredentials(accessKey: "dummy", secretKey: "dummy"));

        if (grarOptions.Wolverine.OptimizeArtifactWorkflow)
            options.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;

        if (grarOptions.Wolverine.AutoProvision)
            transportConfiguration.AutoProvision();

        ConfigureAddressMatchPublisher(options, grarOptions.Sqs.AddressMatchQueueName);

        ConfigureAddressMatchListener(options, grarOptions.Sqs.AddressMatchQueueName,
                                      grarOptions.Sqs.AddressMatchDeadLetterQueueName);

        ConfigureGrarSyncListener(options, grarOptions.Sqs.GrarSyncQueueName, grarOptions.Sqs.GrarSyncDeadLetterQueueName,
                                  grarOptions.Sqs.GrarSyncQueueListenerEnabled);

        ConfigureKboSyncPublisher(options, configuration.GetAppSettings());
    }

    private static void ConfigureKboSyncPublisher(WolverineOptions options, AppSettings appSettings)
    {
        options.Discovery.IncludeType<TeSynchroniserenKboNummerMessage>();

        options.PublishMessage<TeSynchroniserenKboNummerMessage>()
               .ToSqsQueue(appSettings.KboSyncQueueName)
               .InteropWithCloudEvents()
               .MessageBatchMaxDegreeOfParallelism(1)
               .UseDurableOutbox()
               .MessageBatchSize(1);
    }

    private static void ConfigureAddressMatchPublisher(WolverineOptions options, string sqsQueueName)
    {
        options.Discovery.IncludeType<ProbeerAdresTeMatchenCommand>();
        options.Discovery.IncludeType<ProbeerAdresTeMatchenCommandHandler>();

        options.PublishMessage<ProbeerAdresTeMatchenCommand>()
               .ToSqsQueue(sqsQueueName)
               .UseDurableOutbox()
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
               .UseDurableInbox()
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

        options.Discovery.IncludeType<OverkoepelendeGrarConsumerMessage>();
        options.Discovery.IncludeType<OverkoepelendeGrarConsumerMessageHandler>();

        options.PublishMessage<OverkoepelendeGrarConsumerMessage>()
               .ToSqsQueue(sqsQueueName)
               .UseDurableOutbox();

        options.ListenToSqsQueue(sqsQueueName, configure: configure =>
                {
                    configure.MaxNumberOfMessages = 1;
                    configure.DeadLetterQueueName = sqsDeadLetterQueueName;
                })
               .UseDurableInbox()
               .ConfigureDeadLetterQueue(sqsDeadLetterQueueName, configure: queue =>
                {
                    queue.DeadLetterQueueName = sqsDeadLetterQueueName;
                })
               .MaximumParallelMessages(1);
    }
}
