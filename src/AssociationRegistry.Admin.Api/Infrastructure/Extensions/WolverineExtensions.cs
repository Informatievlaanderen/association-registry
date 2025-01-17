namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Acties.GrarConsumer;
using Amazon.Runtime;
using EventStore;
using Grar.AddressMatch;
using Hosts.Configuration;
using JasperFx.CodeGeneration;
using Kbo;
using Serilog;
using Vereniging;
using Wolverine;
using Wolverine.AmazonSqs;
using Wolverine.ErrorHandling;

public static class WolverineExtensions
{
    public static void AddWolverine(this WebApplicationBuilder builder)
    {
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

                ConfigureKboSyncQueuePublisher(options, context.Configuration["KboSyncQueueUrl"]);

                ConfigureAddressMatchPublisher(options, grarOptions.Sqs.AddressMatchQueueName);

                ConfiguredAddressMatchListener(options, grarOptions.Sqs.AddressMatchQueueName,
                                               grarOptions.Sqs.AddressMatchDeadLetterQueueName);

                ConfigureGrarSyncListener(options, grarOptions.Sqs.GrarSyncQueueName, grarOptions.Sqs.GrarSyncDeadLetterQueueName,
                                          grarOptions.Sqs.GrarSyncQueueListenerEnabled);

                if (grarOptions.Wolverine.AutoProvision)
                    transportConfiguration.AutoProvision();

                options.LogMessageStarting(LogLevel.Trace);

                Log.Logger.Information(messageTemplate: "Wolverine Transport SQS configuration: {@TransportConfig}",
                                       transportConfiguration);
            });
    }

    private static void ConfigureAddressMatchPublisher(WolverineOptions options, string sqsQueueName)
    {
        options.PublishMessage<TeAdresMatchenLocatieMessage>()
               .ToSqsQueue(sqsQueueName)
               .MessageBatchSize(1);
    }

    private static void ConfigureKboSyncQueuePublisher(WolverineOptions options, string sqsQueueName)
    {
        options.PublishMessage<TeSynchroniserenKboNummerMessage>()
               .ToSqsQueue("kbo-sync-production")
               .MessageBatchSize(1);

        options.ListenToSqsQueue("kbo-sync-production", configure: configure =>
                {
                    configure.MaxNumberOfMessages = 1;
                })
               .MaximumParallelMessages(1);
    }

    private static void ConfiguredAddressMatchListener(WolverineOptions options, string sqsQueueName, string sqsDeadLetterQueueName)
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
