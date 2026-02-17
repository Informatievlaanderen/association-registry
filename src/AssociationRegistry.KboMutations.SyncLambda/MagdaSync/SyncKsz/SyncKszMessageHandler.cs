namespace AssociationRegistry.KboMutations.SyncLambda.MagdaSync.SyncKsz;

using DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;
using Microsoft.Extensions.Logging;

public class SyncKszMessageHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly ILogger<SyncKszMessageHandler> _logger;
    private readonly VzerVertegenwoordigerForInszQuery _vzerVertegenwoordigerForInszQuery;

    public SyncKszMessageHandler(
        VzerVertegenwoordigerForInszQuery vzerVertegenwoordigerForInszQuery,
        IAggregateSession aggregateSession,
        ILogger<SyncKszMessageHandler> logger
    )
    {
        _vzerVertegenwoordigerForInszQuery = vzerVertegenwoordigerForInszQuery;
        _aggregateSession = aggregateSession;
        _logger = logger;
    }

    public async Task Handle(
        CommandEnvelope<SyncKszMessage> messageEnvelope,
        CancellationToken cancellationToken = default
    )
    {
        var message = messageEnvelope.Command;
        if (!message.Overleden) // only need to sync if overleden for now
        {
            _logger.LogInformation("Skipping message because this person is not deceased");

            return;
        }

        var vzerVertegenwoordigersForInsz = await _vzerVertegenwoordigerForInszQuery.ExecuteAsync(
            message.Insz,
            cancellationToken
        );

        foreach (var vertegenwoordigerPersoonsgegeven in vzerVertegenwoordigersForInsz)
        {
            _logger.LogInformation($"trying to load vcode: {vertegenwoordigerPersoonsgegeven.VCode}");

            var vereniging = await _aggregateSession.Load<Vereniging>(
                VCode.Create(vertegenwoordigerPersoonsgegeven.VCode),
                messageEnvelope.Metadata,
                allowDubbeleVereniging: true,
                allowVerwijderdeVereniging: true
            );

            vereniging.MarkeerVertegenwoordigerAlsOverleden(vertegenwoordigerPersoonsgegeven.VertegenwoordigerId);

            await _aggregateSession.Save(vereniging, messageEnvelope.Metadata, cancellationToken);

            _logger.LogInformation(
                $"SyncKszMessageHandler marked vertegenwoordiger as deceased with vCode: {vertegenwoordigerPersoonsgegeven.VCode} for vertegenwoordiger: {vertegenwoordigerPersoonsgegeven.VertegenwoordigerId}"
            );
        }

        _logger.LogInformation("SyncKszMessageHandler done");
    }
}
