﻿namespace AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingUitKbo;

using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Framework;
using AssociationRegistry.Kbo;
using AssociationRegistry.Resources;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Microsoft.Extensions.Logging;
using ResultNet;

public class RegistreerVerenigingUitKboCommandHandler
{
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly IMagdaRegistreerInschrijvingService _magdaRegistreerInschrijvingService;
    private readonly ILogger<RegistreerVerenigingUitKboCommandHandler> _logger;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingUitKboCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMagdaGeefVerenigingService magdaGeefVerenigingService,
        IMagdaRegistreerInschrijvingService magdaRegistreerInschrijvingService,
        ILogger<RegistreerVerenigingUitKboCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _magdaGeefVerenigingService = magdaGeefVerenigingService;
        _magdaRegistreerInschrijvingService = magdaRegistreerInschrijvingService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingUitKboCommand> message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingUitKboCommandHandler)} start");

        var command = message.Command;
        var duplicateResult = await CheckForDuplicate(command.KboNummer);

        if (duplicateResult.IsFailure())
            return duplicateResult;

        var geefVerenigingResult = await _magdaGeefVerenigingService.GeefVereniging(command.KboNummer, message.Metadata, cancellationToken);

        if(geefVerenigingResult.IsFailure())
            throw new GeenGeldigeVerenigingInKbo();

        switch (geefVerenigingResult)
        {
            case Result<VerenigingVolgensKbo> verenigingResult:
                await RegistreerInschrijving(command.KboNummer, message.Metadata, cancellationToken);

                var result = await RegistreerVereniging(verenigingResult, message.Metadata, cancellationToken);

                _logger.LogInformation($"Handle {nameof(RegistreerVerenigingUitKboCommandHandler)} end");

                return result;

            default:
                throw new ArgumentOutOfRangeException(nameof(geefVerenigingResult));
        }
    }

    private async Task<Result> CheckForDuplicate(KboNummer kboNumber)
    {
        try
        {
            var duplicateKbo = await _verenigingsRepository.Load(kboNumber);
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
        CancellationToken cancellationToken)
    {
        var vCode = await _vCodeService.GetNext();

        var vereniging = VerenigingMetRechtspersoonlijkheid.Registreer(
            vCode,
            verenigingVolgensKbo);

        var duplicateResult = await CheckForDuplicate(verenigingVolgensKbo.KboNummer);

        if (duplicateResult.IsFailure()) return duplicateResult;

        var result = await _verenigingsRepository.Save(vereniging, messageMetadata, cancellationToken);

        return Result.Success(CommandResult.Create(vCode, result));
    }

    private async Task RegistreerInschrijving(
        KboNummer kboNummer,
        CommandMetadata messageMetadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _magdaRegistreerInschrijvingService.RegistreerInschrijving(
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
