﻿namespace AssociationRegistry.Acties.RegistreerVerenigingUitKbo;

using DuplicateVerenigingDetection;
using Framework;
using Kbo;
using ResultNet;
using Vereniging;
using Vereniging.Exceptions;

public class RegistreerVerenigingUitKboCommandHandler
{
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaGeefVerenigingService _magdaGeefVerenigingService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingUitKboCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMagdaGeefVerenigingService magdaGeefVerenigingService)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _magdaGeefVerenigingService = magdaGeefVerenigingService;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingUitKboCommand> message,
        CancellationToken cancellationToken = default)
    {
        var command = message.Command;

        for (var i = 0; i <= 60; i += 3)
        {
            var duplicateResult = await CheckForDuplicate(command.KboNummer);

            if (duplicateResult.IsFailure()) return duplicateResult;

            var kboLockDocument = await _verenigingsRepository.GetKboNummerLock(command.KboNummer);
            var hasLock = kboLockDocument is not null;

            // Lock door andere instance dus even wachten
            if (hasLock)
            {
                await Task.Delay(3 * 1000, cancellationToken);

                continue;
            }

            try
            {
                await _verenigingsRepository.SetKboNummerLock(command.KboNummer);
                var vereniging = await _magdaGeefVerenigingService.GeefVereniging(command.KboNummer, message.Metadata, cancellationToken);

                if (vereniging.IsFailure()) throw new GeenGeldigeVerenigingInKbo();

                return await RegistreerVereniging(vereniging, message.Metadata, cancellationToken);
            }
            finally
            {
                await _verenigingsRepository.DeleteKboNummerLock(command.KboNummer);
            }
        }

        throw new ApplicationException($"Kan niet langer wachten op lock voor KBO nummer {command.KboNummer}");
    }

    private async Task<Result> CheckForDuplicate(KboNummer kboNummer)
    {
        var duplicateKbo = await _verenigingsRepository.GetVCodeAndNaam(kboNummer);

        return duplicateKbo is not null ? DuplicateKboFound.WithVcode(duplicateKbo.VCode!) : Result.Success();
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
}
