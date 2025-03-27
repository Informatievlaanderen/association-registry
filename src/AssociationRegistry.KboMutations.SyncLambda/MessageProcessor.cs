

namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.SQSEvents;
using AssociationRegistry.Notifications;
using Kbo;
using KboMutations.Configuration;
using KboSyncLambda;
using KboSyncLambda.SyncKbo;
using Logging;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Vereniging;

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
        IMagdaGeefVerenigingService geefOndernemingService,
        IVerenigingsRepository repository,
        INotifier notifier,
        CancellationToken cancellationToken)
    {
        var contextLogger = loggerFactory.CreateLogger<MessageProcessor>();
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileBucketName)}:{_kboSyncConfiguration.MutationFileBucketName}");
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.MutationFileQueueUrl)}:{_kboSyncConfiguration.MutationFileQueueUrl}");
        contextLogger.LogInformation($"{nameof(_kboSyncConfiguration.SyncQueueUrl)}:{_kboSyncConfiguration.SyncQueueUrl}");

        var handler = new SyncKboCommandHandler(
            registreerInschrijvingService,
            geefOndernemingService,
            notifier,
            loggerFactory.CreateLogger<SyncKboCommandHandler>()
            );

        foreach (var record in sqsEvent.Records)
        {
            try
            {
                var message = JsonSerializer.Deserialize<TeSynchroniserenKboNummerMessage>(record.Body);
                await RecordProcessor.TryProcessRecord(contextLogger, repository, cancellationToken, message, handler);
            }
            catch(Exception ex)
            {
                await notifier.Notify(new KboSyncLambdaGefaald(record.Body, ex));

                throw;
            }

        }
    }
}
