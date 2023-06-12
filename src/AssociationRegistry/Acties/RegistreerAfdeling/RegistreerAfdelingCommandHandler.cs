namespace AssociationRegistry.Acties.RegistreerAfdeling;

using DuplicateVerenigingDetection;
using EventStore;
using Framework;
using Vereniging;
using ResultNet;

public class RegistreerAfdelingCommandHandler
{
    private readonly IClock _clock;
    private readonly IVCodeService _vCodeService;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerAfdelingCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        IClock clock)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _clock = clock;
    }

    public async Task<Result> Handle(CommandEnvelope<RegistreerAfdelingCommand> message, CancellationToken cancellationToken = default)
    {
        var command = message.Command;
        if (!command.SkipDuplicateDetection)
        {
            var duplicates = (await _duplicateVerenigingDetectionService.GetDuplicates(command.Naam, command.Locaties)).ToList();
            if (duplicates.Any())
                return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
        }

        var vCode = await _vCodeService.GetNext();

        var vCodeAndNaamMoedervereniging =
            await _verenigingsRepository.GetVCodeAndNaam(message.Command.KboNummerMoedervereniging) ??
            VerenigingsRepository.VCodeAndNaam.Fallback(message.Command.KboNummerMoedervereniging);

        var vereniging = Vereniging.RegistreerAfdeling(
            vCode,
            command.Naam,
            command.KboNummerMoedervereniging,
            vCodeAndNaamMoedervereniging,
            command.KorteNaam,
            command.KorteBeschrijving,
            command.Startdatum,
            command.Contactgegevens,
            command.Locaties,
            message.Command.Vertegenwoordigers,
            command.HoofdactiviteitenVerenigingsloket,
            _clock);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata, cancellationToken);
        return Result.Success(CommandResult.Create(vCode, result));
    }
}
