namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using ContactInfo;
using DuplicateDetection;
using Framework;
using Hoofdactiviteiten;
using KboNummers;
using Locaties;
using Magda;
using ResultNet;
using Startdatums;
using VCodes;
using VerenigingsNamen;
using Vertegenwoordigers;

public class RegistreerVerenigingCommandHandler
{
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaFacade _magdaFacade;
    private readonly IDuplicateDetectionService _duplicateDetectionService;
    private readonly IClock _clock;

    public RegistreerVerenigingCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMagdaFacade magdaFacade,
        IDuplicateDetectionService duplicateDetectionService,
        IClock clock)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _magdaFacade = magdaFacade;
        _duplicateDetectionService = duplicateDetectionService;
        _clock = clock;
    }

    public async Task<Result> Handle(CommandEnvelope<RegistreerVerenigingCommand> message, CancellationToken cancellationToken)
    {
        var command = message.Command;
        var naam = new VerenigingsNaam(command.Naam);
        var kboNummer = KboNummer.Create(command.KboNummber);
        var startdatum = StartDatum.Create(_clock, command.Startdatum);
        var locatieLijst = LocatieLijst.CreateInstance(command.Locaties!.Select(ToLocatie));
        var contactInfoLijst = ContactLijst.Create(command.ContactInfoLijst);
        var hoofdactiviteitenVerenigingsloketLijst = HoofdactiviteitenVerenigingsloketLijst.Create(command.HoofdactiviteitenVerenigingsloket.Select(HoofdactiviteitVerenigingsloket.Create));

        if (!message.Command.SkipDuplicateDetection)
        {
            var duplicates = (await _duplicateDetectionService.GetDuplicates(naam, locatieLijst)).ToList();
            if (duplicates.Any())
                return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
        }

        var vertegenwoordigerService = new VertegenwoordigerService(_magdaFacade);
        var vertegenwoordigersLijst = await vertegenwoordigerService.GetVertegenwoordigersLijst(command.Vertegenwoordigers);

        var vCode = await _vCodeService.GetNext();

        var vereniging = Vereniging.Registreer(
            vCode,
            naam,
            command.KorteNaam,
            command.KorteBeschrijving,
            startdatum,
            kboNummer,
            contactInfoLijst,
            locatieLijst,
            vertegenwoordigersLijst,
            hoofdactiviteitenVerenigingsloketLijst,
            _clock.Today);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata);
        return Result.Success(CommandResult.Create(vCode, result));
    }

    private static Locatie ToLocatie(RegistreerVerenigingCommand.Locatie loc)
        => Locatie.CreateInstance(
            loc.Naam,
            loc.Straatnaam,
            loc.Huisnummer,
            loc.Busnummer,
            loc.Postcode,
            loc.Gemeente,
            loc.Land,
            loc.Hoofdlocatie,
            loc.Locatietype);
}
