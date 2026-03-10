namespace AssociationRegistry.CommandHandling.MagdaSync.SyncKsz;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AssociationRegistry.MartenDb.Store;
using Bewaartermijnen.Acties.Start;
using Exceptions;
using Microsoft.Extensions.Logging;
using Wolverine.Marten;

public interface ISyncKszMessageHandler
{
    Task Handle(
        CommandEnvelope<SyncKszMessage> messageEnvelope,
        IMartenOutbox outbox,
        CancellationToken cancellationToken = default
    );
}

public class SyncKszMessageHandler : ISyncKszMessageHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IMagdaGeefPersoonService _magdaGeefPersoonService;
    private readonly ILogger<SyncKszMessageHandler> _logger;
    private readonly VzerVertegenwoordigerForInszQuery _vzerVertegenwoordigerForInszQuery;

    public SyncKszMessageHandler(
        VzerVertegenwoordigerForInszQuery vzerVertegenwoordigerForInszQuery,
        IAggregateSession aggregateSession,
        IMagdaGeefPersoonService magdaGeefPersoonService,
        ILogger<SyncKszMessageHandler> logger
    )
    {
        _vzerVertegenwoordigerForInszQuery = vzerVertegenwoordigerForInszQuery;
        _aggregateSession = aggregateSession;
        _magdaGeefPersoonService = magdaGeefPersoonService;
        _logger = logger;
    }

    public async Task Handle(
        CommandEnvelope<SyncKszMessage> messageEnvelope,
        IMartenOutbox outbox,
        CancellationToken cancellationToken = default
    )
    {
        var message = messageEnvelope.Command;

        var persoonUitKsz = await _magdaGeefPersoonService.GeefPersoon(
            new GeefPersoonRequest(message.Insz),
            messageEnvelope.Metadata,
            cancellationToken
        );

        if (!persoonUitKsz.Overleden)
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
            var vertegenwoordigerId = vertegenwoordigerPersoonsgegeven.VertegenwoordigerId;

            try
            {
                var vCode = VCode.Create(vertegenwoordigerPersoonsgegeven.VCode!);

                var vereniging = await _aggregateSession.Load<Vereniging>(
                    vCode,
                    messageEnvelope.Metadata,
                    allowDubbeleVereniging: true,
                    allowVerwijderdeVereniging: true
                );

                vereniging.MarkeerVertegenwoordigerAlsOverleden(vertegenwoordigerId);

                await outbox.SendAsync(
                    new CommandEnvelope<StartBewaartermijnMessage>(
                        new StartBewaartermijnMessage(
                            vCode,
                            BewaartermijnType.Vertegenwoordigers.Value,
                            vertegenwoordigerId,
                            BewaartermijnReden.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden
                        ),
                        messageEnvelope.Metadata
                    )
                );

                await _aggregateSession.Save(vereniging, messageEnvelope.Metadata, cancellationToken);

                _logger.LogInformation(
                    $"SyncKszMessageHandler marked vertegenwoordiger as deceased with vCode: {vertegenwoordigerPersoonsgegeven.VCode} for vertegenwoordiger: {vertegenwoordigerId}"
                );
            }
            catch (Exception e)
            {
                throw new KszSyncException(
                    vertegenwoordigerPersoonsgegeven.VCode!,
                    vertegenwoordigerId,
                    e
                );
            }
        }

        _logger.LogInformation("SyncKszMessageHandler done");
    }
}
