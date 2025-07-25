﻿namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Amazon.Runtime;
using AssociationRegistry.Middleware;
using DecentraalBeheer.Registratie.RegistreerVerenigingUitKbo;
using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using EventStore;
using Framework;
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
            (options) =>
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

                options.Policies.ForMessagesOfType<CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>>()
                       .AddMiddleware(typeof(EnrichLocatiesMiddleware))
                       .AddMiddleware(typeof(DuplicateDetectionMiddleware));

                var grarOptions = builder.Configuration.GetGrarOptions();

                if (grarOptions.Wolverine.OptimizeArtifactWorkflow)
                    options.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;

                var transportConfiguration = options.UseAmazonSqsTransport(config =>
                {
                    Log.Logger.Information(messageTemplate: "Wolverine SQS configuration: {@Config}", config);
                    config.ServiceURL = grarOptions.Wolverine.TransportServiceUrl;
                });

                if (grarOptions.Sqs.UseLocalStack)
                    transportConfiguration.Credentials(new BasicAWSCredentials(accessKey: "dummy", secretKey: "dummy"));

                ConfigureSqsQueues(options, grarOptions, builder.Configuration.GetAppSettings());

                ConfigurePostgresQueues(options, wolverineSchema, builder.Configuration);

                if (grarOptions.Wolverine.AutoProvision)
                    transportConfiguration.AutoProvision();

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
        string wolverineSchema,
        ConfigurationManager configuration)
    {
        const string AanvaardDubbeleVerenigingQueueName = "aanvaard-dubbele-vereniging-queue";

        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessage>();
        options.Discovery.IncludeType<AanvaardDubbeleVerenigingMessageHandler>();

        var connectionString = configuration.GetPostgreSqlOptionsSection().GetConnectionString();

        options.PersistMessagesWithPostgresql(connectionString, wolverineSchema).EnableMessageTransport();

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
