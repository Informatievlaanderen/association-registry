namespace AssociationRegistry.CommandHandling.KboSyncLambda.SyncKbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Resources;
using Integrations.Magda;
using Integrations.Slack;
using Magda;
using Messages;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using ResultNet;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SyncKboCommandHandler
{
    private readonly IMagdaRegistreerInschrijvingService _registreerInschrijvingService;
    private readonly INotifier _notifier;
    private readonly ILogger<SyncKboCommandHandler> _logger;
    private readonly IMagdaSyncGeefVerenigingService _geefVerenigingService;

    public SyncKboCommandHandler(
        IMagdaRegistreerInschrijvingService registreerInschrijvingService,
        IMagdaSyncGeefVerenigingService geefVerenigingService,
        INotifier notifier,
        ILogger<SyncKboCommandHandler> logger)
    {
        _registreerInschrijvingService = registreerInschrijvingService;
        _geefVerenigingService = geefVerenigingService;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task<CommandResult?> Handle(
        CommandEnvelope<SyncKboCommand> message,
        IVerenigingsRepository repository,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(SyncKboCommandHandler)} start");

        using var scope = KboSyncMetrics.Start(message.Command.KboNummer);

        if (!await repository.Exists(message.Command.KboNummer))
        {
            scope.Dropped();
            return null;
        }

        var verenigingVolgensMagda =
            await _geefVerenigingService.GeefVereniging(message.Command.KboNummer, message.Metadata, cancellationToken);

        if (verenigingVolgensMagda.IsFailure())
        {
            scope.Failed();
            await _notifier.Notify(new KboSynchronisatieMisluktNotification(message.Command.KboNummer));

            throw new GeenGeldigeVerenigingInKbo();
        }

        var vereniging = await repository.Load(message.Command.KboNummer, message.Metadata);

        scope.UseVCode(vereniging.VCode);

       await RegistreerInschrijving(message.Command.KboNummer, message.Metadata, cancellationToken);

        vereniging.NeemGegevensOverUitKboSync(verenigingVolgensMagda);

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);

        _logger.LogInformation($"Handle {nameof(SyncKboCommandHandler)} end");

        scope.Succeed();

        return CommandResult.Create(VCode.Create(vereniging.VCode), result);
    }

    private async Task RegistreerInschrijving(
        KboNummer kboNummer,
        CommandMetadata messageMetadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _registreerInschrijvingService.RegistreerInschrijving(
                kboNummer, messageMetadata, cancellationToken);

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
