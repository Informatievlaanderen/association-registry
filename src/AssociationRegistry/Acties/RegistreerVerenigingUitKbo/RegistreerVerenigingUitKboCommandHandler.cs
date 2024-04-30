﻿namespace AssociationRegistry.Acties.RegistreerVerenigingUitKbo;

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
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly IMagdaRegistreerInschrijvingService _magdaRegistreerInschrijvingService;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly ILogger<RegistreerVerenigingUitKboCommandHandler> _logger;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingUitKboCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMagdaGeefVerenigingService magdaGeefVerenigingService,
        IMagdaRegistreerInschrijvingService magdaRegistreerInschrijvingService,
        IMartenOutbox outbox,
        IDocumentSession session,
        ILogger<RegistreerVerenigingUitKboCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _magdaGeefVerenigingService = magdaGeefVerenigingService;
        _magdaRegistreerInschrijvingService = magdaRegistreerInschrijvingService;
        _outbox = outbox;
        _session = session;
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

        await RegistreerInschrijving(command.KboNummer, message.Metadata, cancellationToken);

        var geefVerenigingResult = await _magdaGeefVerenigingService.GeefVereniging(command.KboNummer, message.Metadata, cancellationToken);

        if (geefVerenigingResult.IsFailure() || !geefVerenigingResult.Data.IsActief)
            throw new GeenGeldigeVerenigingInKbo();

        var result = await RegistreerVereniging(geefVerenigingResult, message.Metadata, cancellationToken);

        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingUitKboCommandHandler)} end");

        return result;
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
