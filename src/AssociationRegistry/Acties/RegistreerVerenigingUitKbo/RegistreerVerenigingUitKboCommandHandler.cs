namespace AssociationRegistry.Acties.RegistreerVerenigingUitKbo;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using DuplicateVerenigingDetection;
using Events;
using Framework;
using Grar.AddressMatch;
using Kbo;
using Marten;
using Microsoft.Extensions.Logging;
using Resources;
using ResultNet;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine.Marten;

public class RegistreerVerenigingUitKboCommandHandler
{
    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingUitKboCommand> message,

        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMagdaGeefVerenigingService magdaGeefVerenigingService,
        IMagdaRegistreerInschrijvingService magdaRegistreerInschrijvingService,
        ILogger<RegistreerVerenigingUitKboCommandHandler> logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Handle {nameof(RegistreerVerenigingUitKboCommandHandler)} start");

        var command = message.Command;
        var duplicateResult = await CheckForDuplicate(command.KboNummer, verenigingsRepository);

        if (duplicateResult.IsFailure())
            return duplicateResult;

        await RegistreerInschrijving(command.KboNummer, message.Metadata, magdaRegistreerInschrijvingService, logger, cancellationToken);

        var geefVerenigingResult = await magdaGeefVerenigingService.GeefVereniging(command.KboNummer, message.Metadata, cancellationToken);

        if (geefVerenigingResult.IsFailure() || !geefVerenigingResult.Data.IsActief)
            throw new GeenGeldigeVerenigingInKbo();

        var result = await RegistreerVereniging(geefVerenigingResult, message.Metadata, verenigingsRepository, vCodeService, cancellationToken);

        logger.LogInformation($"Handle {nameof(RegistreerVerenigingUitKboCommandHandler)} end");

        return result;
    }

    private async Task<Result> CheckForDuplicate(KboNummer kboNumber, IVerenigingsRepository verenigingsRepository)
    {
        try
        {
            var duplicateKbo = await verenigingsRepository.Load(kboNumber);
            return DuplicateKboFound.WithVcode(duplicateKbo.VCode!);
        }
        catch (AggregateNotFoundException)
        {
            return Result.Success();
        }
    }

    private async Task<Result> RegistreerVereniging(
        VerenigingVolgensKbo verenigingVolgensKbo,
        CommandMetadata messageMetadata,
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        CancellationToken cancellationToken)
    {
        var vCode = await vCodeService.GetNext();

        var vereniging = VerenigingMetRechtspersoonlijkheid.Registreer(
            vCode,
            verenigingVolgensKbo);

        var duplicateResult = await CheckForDuplicate(verenigingVolgensKbo.KboNummer, verenigingsRepository);

        if (duplicateResult.IsFailure()) return duplicateResult;

        var result = await verenigingsRepository.Save(vereniging, messageMetadata, cancellationToken);

        return Result.Success(CommandResult.Create(vCode, result));
    }

    private async Task RegistreerInschrijving(
        KboNummer kboNummer,
        CommandMetadata messageMetadata,
        IMagdaRegistreerInschrijvingService magdaRegistreerInschrijvingService,
        ILogger<RegistreerVerenigingUitKboCommandHandler> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await magdaRegistreerInschrijvingService.RegistreerInschrijving(
                kboNummer, messageMetadata, cancellationToken);

            if (result.IsFailure())
                throw new RegistreerInschrijvingKonNietVoltooidWorden();

            logger.LogInformation(LoggerMessages.KboRegistreerInschrijvingGeslaagd, kboNummer);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, LoggerMessages.KboRegistreerInschrijvingNietGeslaagd, kboNummer);

            throw new RegistreerInschrijvingKonNietVoltooidWorden();
        }
    }
}
