namespace AssociationRegistry.CommandHandling.MagdaSync.SyncKbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Slack;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.OpenTelemetry.Metrics;
using AssociationRegistry.Resources;
using Microsoft.Extensions.Logging;
using Notifications;
using ResultNet;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class SyncKboCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IMagdaRegistreerInschrijvingService _registreerInschrijvingService;
    private readonly INotifier _notifier;
    private readonly ILogger<SyncKboCommandHandler> _logger;
    private readonly IMagdaSyncGeefVerenigingService _geefVerenigingService;

    public SyncKboCommandHandler(
        IVerenigingsRepository repository,
        IMagdaRegistreerInschrijvingService registreerInschrijvingService,
        IMagdaSyncGeefVerenigingService geefVerenigingService,
        INotifier notifier,
        ILogger<SyncKboCommandHandler> logger)
    {
        _repository = repository;
        _registreerInschrijvingService = registreerInschrijvingService;
        _geefVerenigingService = geefVerenigingService;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task Handle(
        CommandEnvelope<SyncKboCommand> message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(SyncKboCommandHandler)} start");

        using var scope = KboSyncMetrics.Start(message.Command.KboNummer);

        if (!await _repository.Exists(message.Command.KboNummer))
        {
            scope.Dropped();
            throw new InvalidOperationException("KboNummer not found");
        }

        var verenigingVolgensMagda =
            await _geefVerenigingService.GeefVereniging(message.Command.KboNummer, AanroependeFunctie.SyncKbo, message.Metadata, cancellationToken);

        if (verenigingVolgensMagda.IsFailure())
        {
            scope.Failed();
            await _notifier.Notify(new KboSynchronisatieMisluktNotification(message.Command.KboNummer));

            throw new GeenGeldigeVerenigingInKbo();
        }

        var vereniging = await _repository.Load(message.Command.KboNummer, message.Metadata);

        scope.UseVCode(vereniging.VCode);

       await RegistreerInschrijving(message.Command.KboNummer, message.Metadata, cancellationToken);

        vereniging.NeemGegevensOverUitKboSync(verenigingVolgensMagda);

        await _repository.Save(vereniging, message.Metadata, cancellationToken);

        _logger.LogInformation($"Handle {nameof(SyncKboCommandHandler)} end");

        scope.Succeed();
    }

    private async Task RegistreerInschrijving(
        KboNummer kboNummer,
        CommandMetadata messageMetadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _registreerInschrijvingService.RegistreerInschrijving(
                kboNummer, AanroependeFunctie.SyncKbo, messageMetadata, cancellationToken);

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
