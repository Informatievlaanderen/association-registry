

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

        var handler = new SyncKboCommandHandler(
            repository,
            registreerInschrijvingService,
            geefOndernemingService,
            notifier,
            logger);

        var kszSyncHandler = new SyncVertegenwoordigerCommandHandler(
            repository,
            geefPersoonService,
            loggerFactory.CreateLogger<SyncVertegenwoordigerCommandHandler>());

        foreach (var record in sqsEvent.Records)
        {
            try
            {
                contextLogger.LogInformation("Processing record {Id}", record.MessageId);
                contextLogger.LogInformation("Message attributes for record {Id}", string.Join("\n", record.MessageAttributes.Select(k => $"{k.Key}:{k.Value.StringValue}")));
                contextLogger.LogInformation("Record attributes for record {Id}", string.Join("\n", record.Attributes.Select(k => $"{k.Key}:{k.Value.ToString()}")));

                var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;


                using var doc = JsonDocument.Parse(record.Body);
                var root = doc.RootElement;

                IMagdaSyncHandler[] handlers =
                [
                    handler, kszSyncHandler,
                ];

                if(root.TryGetProperty("Insz", out var insz)
                     && insz.ValueKind == JsonValueKind.String)
                {
                    var commandEnvelope = new CommandEnvelope<TCommand>(command, commandMetadata);


                }
                else if(root.TryGetProperty("KboNummer", out var kboEl)
                     && kboEl.ValueKind == JsonValueKind.String)
                {
                    var kboNummerValue = kboEl.GetString();

                    if (string.IsNullOrWhiteSpace(kboNummerValue))
                        throw new JsonException("KboNummer is empty");

                    var syncKboCommand = new SyncKboCommand(
                        KboNummer.Create(kboNummerValue)
                    );

                    await RecordProcessor.TryProcessRecord(
                        contextLogger,
                        repository,
                        cancellationToken,
                        syncKboCommand,
                        handler
                    );
                }

            }
            catch(Exception ex)
            {
                await notifier.Notify(new KboSyncLambdaGefaald(record.Body, ex));

                throw;
            }
        }
    }
}
