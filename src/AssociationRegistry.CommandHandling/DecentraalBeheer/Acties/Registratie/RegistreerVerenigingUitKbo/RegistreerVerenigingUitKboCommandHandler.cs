namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingUitKbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Persoonsgegevens;
using AssociationRegistry.Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Integrations.Magda;
using Integrations.Magda.GeefOnderneming;
using Magda;
using Marten;
using Microsoft.Extensions.Logging;
using RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using ResultNet;
using System;
using System.Threading;
using System.Threading.Tasks;

public class RegistreerVerenigingUitKboCommandHandler
{
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly IMagdaRegistreerInschrijvingService _magdaRegistreerInschrijvingService;
    private readonly IDocumentSession _session;
    private readonly ILogger<RegistreerVerenigingUitKboCommandHandler> _logger;
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IVertegenwoordigerPersoonsgegevensService _service;

    public RegistreerVerenigingUitKboCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVertegenwoordigerPersoonsgegevensService service,
        IVCodeService vCodeService,
        IMagdaGeefVerenigingService geefVerenigingService,
        IMagdaRegistreerInschrijvingService magdaRegistreerInschrijvingService,
        IDocumentSession session,
        ILogger<RegistreerVerenigingUitKboCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _service = service;
        _vCodeService = vCodeService;
        _magdaGeefVerenigingService = geefVerenigingService;
        _magdaRegistreerInschrijvingService = magdaRegistreerInschrijvingService;
        _session = session;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingUitKboCommand> message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingUitKboCommandHandler)} start");

        var command = message.Command;
        var duplicateResult = await CheckForDuplicate(command.KboNummer, message.Metadata);

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

    private async Task<Result> CheckForDuplicate(KboNummer kboNumber, CommandMetadata metadata)
    {
        try
        {
            var duplicateKbo = await _verenigingsRepository.Load(kboNumber, metadata);
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

        var duplicateResult = await CheckForDuplicate(verenigingVolgensKbo.KboNummer, messageMetadata);

        if (duplicateResult.IsFailure()) return duplicateResult;

        var result = await _verenigingsRepository.SaveNew(vereniging, _session, messageMetadata, cancellationToken);

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
