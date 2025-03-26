namespace AssociationRegistry.KboSyncLambda.SyncKbo;

using AssociationRegistry.Framework;
using AssociationRegistry.Kbo;
using AssociationRegistry.Notifications;
using AssociationRegistry.Notifications.Messages;
using AssociationRegistry.Resources;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using Microsoft.Extensions.Logging;
using ResultNet;

public class SyncKboCommandHandler
{
    private readonly IMagdaRegistreerInschrijvingService _registreerInschrijvingService;
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly INotifier _notifier;
    private readonly ILogger<SyncKboCommandHandler> _logger;

    public SyncKboCommandHandler(
        IMagdaRegistreerInschrijvingService registreerInschrijvingService,
        IMagdaGeefVerenigingService magdaGeefVerenigingService,
        INotifier notifier,
        ILogger<SyncKboCommandHandler> logger)
    {
        _registreerInschrijvingService = registreerInschrijvingService;
        _magdaGeefVerenigingService = magdaGeefVerenigingService;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task<CommandResult?> Handle(
        CommandEnvelope<SyncKboCommand> message,
        IVerenigingsRepository repository,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(SyncKboCommandHandler)} start");

        if (!await repository.Exists(message.Command.KboNummer))
        {
            return null;
        }

        var vereniging = await repository.Load(message.Command.KboNummer, message.Metadata.ExpectedVersion);

        await RegistreerInschrijving(message.Command.KboNummer, message.Metadata, cancellationToken);

        var verenigingVolgensMagda =
            await _magdaGeefVerenigingService.GeefSyncVereniging(message.Command.KboNummer, message.Metadata, cancellationToken);

        if (verenigingVolgensMagda.IsFailure())
        {
            await _notifier.Notify(new KboSynchronisatieMisluktMessage(message.Command.KboNummer));

            throw new GeenGeldigeVerenigingInKbo();
        }

        vereniging.NeemGegevensOverUitKboSync(verenigingVolgensMagda);

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);

        _logger.LogInformation($"Handle {nameof(SyncKboCommandHandler)} end");

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
