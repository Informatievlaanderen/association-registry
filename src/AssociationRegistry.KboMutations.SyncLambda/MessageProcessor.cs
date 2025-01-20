

namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.SQSEvents;
using KboMutations.Configuration;
using Logging;
using Microsoft.Extensions.Logging;
using AssociationRegistry.Kbo;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Notifications;
using AssociationRegistry.KboSyncLambda.SyncKbo;
using AssociationRegistry.Framework;
using NodaTime;
using System.Text.Json;

public class MessageProcessor
{
    private readonly KboSyncConfiguration _kboSyncConfiguration;
    private const string Initiator = "OVO002949";

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
            await TryProcessRecord(contextLogger, repository, notifier, cancellationToken, record, handler);
        }
    }

    private static async Task TryProcessRecord(ILogger contextLogger, IVerenigingsRepository repository,
        INotifier notifier, CancellationToken cancellationToken, SQSEvent.SQSMessage record, SyncKboCommandHandler handler)
    {
        try
        {
            var message = JsonSerializer.Deserialize<TeSynchroniserenKboNummerMessage>(record.Body);

            contextLogger.LogInformation($"Processing record: {message.KboNummer}");

            var syncKboCommand = new SyncKboCommand(KboNummer.Create(message.KboNummer));
            var commandMetadata = new CommandMetadata(Initiator, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(), null);
            var commandEnvelope = new CommandEnvelope<SyncKboCommand>(syncKboCommand, commandMetadata);

            var commandResult = await handler.Handle(commandEnvelope, repository, cancellationToken);

            contextLogger.LogInformation($"Sync resulted in sequence '{commandResult.Sequence}'. HasChanges? {commandResult.HasChanges()}");
        }
        catch(Exception ex)
        {
            await notifier.Notify(new KboSyncLambdaGefaald(record.Body, ex));

            throw;
        }
    }
}
