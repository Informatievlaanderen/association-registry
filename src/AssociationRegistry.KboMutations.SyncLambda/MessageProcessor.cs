

namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.SQSEvents;
using AssociationRegistry.Notifications;
using Kbo;
using KboMutations.Configuration;
using KboSyncLambda;
using KboSyncLambda.SyncKbo;
using Logging;
using Magda.Models;
using Microsoft.Extensions.Logging;
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
                var message = SerializeWithOrWithoutEnvelope(record);
                await RecordProcessor.TryProcessRecord(contextLogger, repository, cancellationToken, message, handler);
            }
            catch(Exception ex)
            {
                await notifier.Notify(new KboSyncLambdaGefaald(record.Body, ex));

                throw;
            }
        }
    }

    private static Envelope SerializeWithOrWithoutEnvelope(SQSEvent.SQSMessage record)
    {
        try
        {
            var withoutEnvelope = JsonSerializer.Deserialize<TeSynchroniserenKboNummerMessage>(record.Body);

            return new Envelope(withoutEnvelope!);

        }catch(Exception ex)
        {
            var withEnvelope = JsonSerializer.Deserialize<Envelope>(record.Body);

            return withEnvelope!;
        }
    }
}
