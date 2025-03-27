namespace AssociationRegistry.KboSyncLambda;

using Framework;
using Kbo;
using Microsoft.Extensions.Logging;
using NodaTime;
using SyncKbo;
using Vereniging;

public class RecordProcessor
{
    private const string Initiator = "OVO002949";

    public static async Task TryProcessRecord(
        ILogger contextLogger,
        IVerenigingsRepository repository,
        CancellationToken cancellationToken,
        TeSynchroniserenKboNummerMessage? message,
        SyncKboCommandHandler handler)
    {
        contextLogger.LogInformation($"Processing record: {message.KboNummer}");

        var syncKboCommand = new SyncKboCommand(KboNummer.Create(message.KboNummer));
        var commandMetadata = new CommandMetadata(Initiator, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(), null);
        var commandEnvelope = new CommandEnvelope<SyncKboCommand>(syncKboCommand, commandMetadata);

        var commandResult = await handler.Handle(commandEnvelope, repository, cancellationToken);

        if (commandResult is null)
        {
            contextLogger.LogInformation("Sync resulted in nothing to sync.");

            return;
        }

        contextLogger.LogInformation($"Sync resulted in sequence '{commandResult.Sequence}'. HasChanges? {commandResult.HasChanges()}");
    }
}
