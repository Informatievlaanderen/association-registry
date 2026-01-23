namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingUitKbo;

using System;
using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Integrations.Magda;
using Microsoft.Extensions.Logging;
using RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using ResultNet;

public class RegistreerVerenigingUitKboCommandHandler
{
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly IMagdaRegistreerInschrijvingService _magdaRegistreerInschrijvingService;
    private readonly ILogger<RegistreerVerenigingUitKboCommandHandler> _logger;
    private readonly INewAggregateSession _aggregateSession;
    private readonly IVerenigingStateQueryService _verenigingStateQueryService;

    public RegistreerVerenigingUitKboCommandHandler(
        INewAggregateSession aggregateSession,
        IVerenigingStateQueryService verenigingStateQueryService,
        IVCodeService vCodeService,
        IMagdaGeefVerenigingService geefVerenigingService,
        IMagdaRegistreerInschrijvingService magdaRegistreerInschrijvingService,
        ILogger<RegistreerVerenigingUitKboCommandHandler> logger
    )
    {
        _aggregateSession = aggregateSession;
        _verenigingStateQueryService = verenigingStateQueryService;
        _vCodeService = vCodeService;
        _magdaGeefVerenigingService = geefVerenigingService;
        _magdaRegistreerInschrijvingService = magdaRegistreerInschrijvingService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingUitKboCommand> message,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingUitKboCommandHandler)} start");

        var command = message.Command;
        var duplicateResult = await CheckForDuplicate(command.KboNummer);

        if (duplicateResult.IsFailure())
            return duplicateResult;

        var aanroependeFunctie = AanroependeFunctie.RegistreerVerenigingMetRechtspersoonlijkheid;
        var geefVerenigingResult = await _magdaGeefVerenigingService.GeefVereniging(
            command.KboNummer,
            aanroependeFunctie,
            message.Metadata,
            cancellationToken
        );

        if (geefVerenigingResult.IsFailure())
            throw new GeenGeldigeVerenigingInKbo();

        switch (geefVerenigingResult)
        {
            case Result<VerenigingVolgensKbo> verenigingResult:
                await RegistreerInschrijving(
                    command.KboNummer,
                    aanroependeFunctie,
                    message.Metadata,
                    cancellationToken
                );

                var result = await RegistreerVereniging(verenigingResult, message.Metadata, cancellationToken);

                _logger.LogInformation($"Handle {nameof(RegistreerVerenigingUitKboCommandHandler)} end");

                return result;

            default:
                throw new ArgumentOutOfRangeException(nameof(geefVerenigingResult));
        }
    }

    private async Task<Result> CheckForDuplicate(KboNummer kboNumber)
    {
        var existingVCode = await _verenigingStateQueryService.GetOptionalVCodeFor(kboNumber);

        if (existingVCode != null)
            return DuplicateKboFound.WithVcode(existingVCode);

        return Result.Success();
    }

    private async Task<Result> RegistreerVereniging(
        VerenigingVolgensKbo verenigingVolgensKbo,
        CommandMetadata messageMetadata,
        CancellationToken cancellationToken
    )
    {
        var vCode = await _vCodeService.GetNext();

        var vereniging = VerenigingMetRechtspersoonlijkheid.Registreer(vCode, verenigingVolgensKbo);

        var duplicateResult = await CheckForDuplicate(verenigingVolgensKbo.KboNummer);

        if (duplicateResult.IsFailure())
            return duplicateResult;

        var result = await _aggregateSession.SaveNew(vereniging, messageMetadata, cancellationToken);

        return Result.Success(CommandResult.Create(vCode, result));
    }

    private async Task RegistreerInschrijving(
        KboNummer kboNummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata messageMetadata,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await _magdaRegistreerInschrijvingService.RegistreerInschrijving(
                kboNummer,
                aanroependeFunctie,
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
