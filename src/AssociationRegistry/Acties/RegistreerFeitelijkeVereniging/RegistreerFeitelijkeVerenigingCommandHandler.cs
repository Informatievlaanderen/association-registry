namespace AssociationRegistry.Acties.RegistreerFeitelijkeVereniging;

using AddressMatch.Messages;
using DuplicateVerenigingDetection;
using Framework;
using ResultNet;
using Vereniging;
using Wolverine.Marten;

public class RegistreerFeitelijkeVerenigingCommandHandler
{
    private readonly IClock _clock;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IMartenOutbox _outbox;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerFeitelijkeVerenigingCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        IMartenOutbox outbox,
        IClock clock)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _outbox = outbox;
        _clock = clock;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerFeitelijkeVerenigingCommand> message,
        CancellationToken cancellationToken = default)
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
            command.Startdatum,
            command.Doelgroep,
            command.IsUitgeschrevenUitPubliekeDatastroom,
            command.Contactgegevens,
            command.Locaties,
            message.Command.Vertegenwoordigers,
            command.HoofdactiviteitenVerenigingsloket,
            _clock);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata, cancellationToken);

        await _outbox.PublishAsync(new TeSynchroniserenAdresMessage(vCode, 1));

        return Result.Success(CommandResult.Create(vCode, result));
    }
}
