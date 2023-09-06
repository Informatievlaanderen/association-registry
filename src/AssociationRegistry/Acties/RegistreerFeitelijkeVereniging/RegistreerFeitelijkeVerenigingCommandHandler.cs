namespace AssociationRegistry.Acties.RegistreerFeitelijkeVereniging;

using DuplicateVerenigingDetection;
using Framework;
using ResultNet;
using Vereniging;

public class RegistreerFeitelijkeVerenigingCommandHandler
{
    private readonly IClock _clock;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerFeitelijkeVerenigingCommandHandler(
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

    public async Task<Result> Handle(CommandEnvelope<RegistreerFeitelijkeVerenigingCommand> message, CancellationToken cancellationToken = default)
    {
        var command = message.Command;

        if (!command.SkipDuplicateDetection)
        {
            var duplicates = (await _duplicateVerenigingDetectionService.GetDuplicates(command.Naam, command.Locaties)).ToList();

            if (duplicates.Any())
                return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
        }

        var vCode = await _vCodeService.GetNext();

        var vereniging = Vereniging.RegistreerFeitelijkeVereniging(
            vCode,
            command.Naam,
            command.KorteNaam,
            command.KorteBeschrijving,
            command.Datum,
            command.Doelgroep,
            command.IsUitgeschrevenUitPubliekeDatastroom,
            command.Contactgegevens,
            command.Locaties,
            message.Command.Vertegenwoordigers,
            command.HoofdactiviteitenVerenigingsloket,
            _clock);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata, cancellationToken);

        return Result.Success(CommandResult.Create(vCode, result));
    }
}
