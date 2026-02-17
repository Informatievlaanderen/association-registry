namespace AssociationRegistry.KboMutations.SyncLambda.MagdaSync.SyncKbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Slack;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.OpenTelemetry.Metrics;
using AssociationRegistry.Resources;
using Exceptions;
using MartenDb.Store;
using Microsoft.Extensions.Logging;
using Notifications;
using ResultNet;

public class SyncKboCommandHandler
{
    private readonly IMagdaRegistreerInschrijvingService _registreerInschrijvingService;
    private readonly INotifier _notifier;
    private readonly ILogger<SyncKboCommandHandler> _logger;
    private readonly IMagdaSyncGeefVerenigingService _geefVerenigingService;
    private readonly KboSyncMetrics _metrics;

    public SyncKboCommandHandler(
        IMagdaRegistreerInschrijvingService registreerInschrijvingService,
        IMagdaSyncGeefVerenigingService geefVerenigingService,
        INotifier notifier,
        ILogger<SyncKboCommandHandler> logger,
        KboSyncMetrics metrics
    )
    {
        _registreerInschrijvingService = registreerInschrijvingService;
        _geefVerenigingService = geefVerenigingService;
        _notifier = notifier;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<CommandResult?> Handle(
        CommandEnvelope<SyncKboCommand> message,
        IAggregateSession aggregateSession,
        IVerenigingStateQueryService verenigingStateQueryService,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation($"Handle {nameof(SyncKboCommandHandler)} start");

        var vCode = "";
        try
        {
            using var scope = _metrics.Start("kbo");

            if (!await verenigingStateQueryService.Exists(message.Command.KboNummer))
            {
                scope.Dropped();
                return null;
            }

            var verenigingVolgensMagda = await _geefVerenigingService.GeefVereniging(
                message.Command.KboNummer,
                AanroependeFunctie.SyncKbo,
                message.Metadata,
                cancellationToken
            );

            if (verenigingVolgensMagda.IsFailure())
            {
                scope.Failed();
                await _notifier.Notify(new KboSynchronisatieMisluktNotification(message.Command.KboNummer));

                throw new GeenGeldigeVerenigingInKbo();
            }

            var vereniging = await aggregateSession.Load(message.Command.KboNummer, message.Metadata);
            vCode = vereniging.VCode;
            await RegistreerInschrijving(message.Command.KboNummer, message.Metadata, cancellationToken);

            vereniging.NeemGegevensOverUitKboSync(verenigingVolgensMagda);

            var result = await aggregateSession.Save(vereniging, message.Metadata, cancellationToken);

            _logger.LogInformation($"Handle {nameof(SyncKboCommandHandler)} end");

            scope.Succeed();

            return CommandResult.Create(VCode.Create(vereniging.VCode), result);
        }
        catch (Exception e)
        {
            throw new KboSyncException(vCode, message.Command.KboNummer, e);
        }
    }

    private async Task RegistreerInschrijving(
        KboNummer kboNummer,
        CommandMetadata messageMetadata,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await _registreerInschrijvingService.RegistreerInschrijving(
                kboNummer,
                AanroependeFunctie.SyncKbo,
                messageMetadata,
                cancellationToken
            );

            if (result.IsFailure())
                throw new RegistreerInschrijvingKonNietVoltooidWorden();

            _logger.LogInformation(LoggerMessages.KboRegistreerInschrijvingGeslaagd, kboNummer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, LoggerMessages.KboRegistreerInschrijvingNietGeslaagd, kboNummer);

            throw new RegistreerInschrijvingKonNietVoltooidWorden();
        }
    }
}
