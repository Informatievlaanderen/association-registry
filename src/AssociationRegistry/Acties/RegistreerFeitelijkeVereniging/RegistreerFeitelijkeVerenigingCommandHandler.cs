namespace AssociationRegistry.Acties.RegistreerFeitelijkeVereniging;

using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AssociationRegistry.Vereniging;
using ResultNet;

public class RegistreerFeitelijkeVerenigingCommandHandler
{
    private readonly IClock _clock;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IMagdaFacade _magdaFacade;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerFeitelijkeVerenigingCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMagdaFacade magdaFacade,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        IClock clock)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _magdaFacade = magdaFacade;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _clock = clock;
    }

    public async Task<Result> Handle(CommandEnvelope<RegistreerFeitelijkeVerenigingCommand> message, CancellationToken cancellationToken)
    {
        var command = message.Command;
        if (!message.Command.SkipDuplicateDetection)
        {
            var duplicates = (await _duplicateVerenigingDetectionService.GetDuplicates(command.Naam, command.Locaties)).ToList();
            if (duplicates.Any())
                return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
        }

        var vertegenwoordigerService = new VertegenwoordigerService(_magdaFacade);
        var vertegenwoordigersLijst = await vertegenwoordigerService.GetVertegenwoordigers(command.Vertegenwoordigers);

        var vCode = await _vCodeService.GetNext();

        var vereniging = Vereniging.Registreer(
            vCode,
            command.Naam,
            command.KorteNaam,
            command.KorteBeschrijving,
            command.Startdatum,
            command.Contactgegevens,
            command.Locaties,
            vertegenwoordigersLijst,
            command.HoofdactiviteitenVerenigingsloket,
            _clock);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata);
        return Result.Success(CommandResult.Create(vCode, result));
    }
}
