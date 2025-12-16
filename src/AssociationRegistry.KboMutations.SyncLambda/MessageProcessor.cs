

namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.SQSEvents;
using CommandHandling.KboSyncLambda.SyncKbo;
using DecentraalBeheer.Vereniging;
using KboMutations.Configuration;
using CommandHandling.KboSyncLambda;
using Integrations.Slack;
using Logging;
using AssociationRegistry.Magda.Kbo;
using CommandHandling.MagdaSync.SyncKsz;
using Contracts.MagdaSync.KboSync;
using Contracts.MagdaSync.KszSync;
using Framework;
using Magda.Persoon;
using Microsoft.Extensions.Logging;
using Notifications;
using System.Text.Json;
using Vereniging;
using Wolverine;

public class MessageProcessor
{
    private readonly KboSyncConfiguration _kboSyncConfiguration;

    public MessageProcessor(KboSyncConfiguration kboSyncConfiguration)
    {
        _kboSyncConfiguration = kboSyncConfiguration;
    }

    public async Task ProcessMessage(SQSEvent sqsEvent,
        ILoggerFactory loggerFactory,
        IMagdaRegistreerInschrijvingService registreerInschrijvingService,
        IMagdaSyncGeefVerenigingService geefOndernemingService,
        IMagdaGeefPersoonService geefPersoonService,
        IVerenigingsRepository repository,
        INotifier notifier,
        CancellationToken cancellationToken)
    {
        var contextLogger = loggerFactory.CreateLogger<MessageProcessor>();
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileBucketName)}:{_kboSyncConfiguration.MutationFileBucketName}");
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileQueueUrl)}:{_kboSyncConfiguration.MutationFileQueueUrl}");
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.SyncQueueUrl)}:{_kboSyncConfiguration.SyncQueueUrl}");

        var logger = loggerFactory.CreateLogger<SyncKboCommandHandler>();

        var kboSyncHandler = new SyncKboCommandHandler(
            repository,
            registreerInschrijvingService,
            geefOndernemingService,
            notifier,
            logger);

        var kszSyncHandler = new SyncVertegenwoordigerCommandHandler(
            repository,
            geefPersoonService,
            loggerFactory.CreateLogger<SyncVertegenwoordigerCommandHandler>());

        IMagdaSyncHandler[] handlers =
        [
            kszSyncHandler,
            kboSyncHandler,
        ];

        foreach (var record in sqsEvent.Records)
        {
            try
            {
                contextLogger.LogInformation("Processing record {Id}", record.MessageId);
                contextLogger.LogInformation("Message attributes for record {Id}", string.Join("\n", record.MessageAttributes.Select(k => $"{k.Key}:{k.Value.StringValue}")));
                contextLogger.LogInformation("Record attributes for record {Id}", string.Join("\n", record.Attributes.Select(k => $"{k.Key}:{k.Value.ToString()}")));

                var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;

                var handler = handlers.SingleOrDefault(x => x.CanHandle(record.Body));

                if (handler == null)
                {
                    contextLogger.LogWarning("No handler found for record {Id}", record.MessageId);
                    throw new InvalidOperationException($"No handler found for message: {record.Body}");
                }

                contextLogger.LogInformation("Handler {Handler} will process record {Id}",
                                             handler.GetType().Name, record.MessageId);

                await handler.Handle(record.Body, commandMetadata, cancellationToken);
            }
            catch(Exception ex)
            {
                await notifier.Notify(new KboSyncLambdaGefaald(record.Body, ex));

                throw;
            }
        }
    }
}
