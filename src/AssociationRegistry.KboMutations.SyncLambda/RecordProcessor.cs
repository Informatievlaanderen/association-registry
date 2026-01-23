namespace AssociationRegistry.KboMutations.SyncLambda;

using System;
using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Contracts.KboSync;
using MagdaSync.SyncKbo;
using MartenDb.Store;
using Microsoft.Extensions.Logging;
using NodaTime;

public class RecordProcessor
{
    private const string Initiator = "OVO002949";

    public static async Task TryProcessRecord(
        ILogger contextLogger,
        IAggregateSession aggregateSession,
        IVerenigingStateQueryService queryService,
        CancellationToken cancellationToken,
        TeSynchroniserenKboNummerMessage? message,
        SyncKboCommandHandler handler
    )
    {
        contextLogger.LogInformation($"Processing record: {message.KboNummer}");

        var syncKboCommand = new SyncKboCommand(KboNummer.Create(message.KboNummer));
        var commandMetadata = new CommandMetadata(
            Initiator,
            SystemClock.Instance.GetCurrentInstant(),
            Guid.NewGuid(),
            null
        );
        var commandEnvelope = new CommandEnvelope<SyncKboCommand>(syncKboCommand, commandMetadata);

        var commandResult = await handler.Handle(commandEnvelope, aggregateSession, queryService, cancellationToken);

        if (commandResult is null)
        {
            return;
        }

        contextLogger.LogInformation(
            $"Sync resulted in sequence '{commandResult.Sequence}'. HasChanges? {commandResult.HasChanges()}"
        );
    }
}
