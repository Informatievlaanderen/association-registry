namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using ContactInfo;
using Framework;
using KboNummers;
using Locaties;
using Magda;
using Startdatums;
using VCodes;
using VerenigingsNamen;
using Vertegenwoordigers;

public class RegistreerVerenigingCommandHandler
{
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IVCodeService _vCodeService;
    private readonly IMagdaFacade _magdaFacade;
    private readonly IClock _clock;

    public RegistreerVerenigingCommandHandler(IVerenigingsRepository verenigingsRepository, IVCodeService vCodeService, IMagdaFacade magdaFacade, IClock clock)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _magdaFacade = magdaFacade;
        _clock = clock;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<RegistreerVerenigingCommand> message, CancellationToken cancellationToken)
    {
        var command = message.Command;
        var naam = new VerenigingsNaam(command.Naam);
        var kboNummer = KboNummer.Create(command.KboNummber);
        var startdatum = Startdatum.Create(_clock, command.Startdatum);
        var locatieLijst = LocatieLijst.CreateInstance(command.Locaties!.Select(ToLocatie));
        var contactInfoLijst = ContactLijst.Create(command.ContactInfoLijst);


        var vertegenwoordigersLijst = VertegenwoordigersLijst.Empty;
        if (command.Vertegenwoordigers is { } vertegenwoordigers)
        {
            var vertegenwoordigersArray = vertegenwoordigers.ToArray();
            if (vertegenwoordigersArray.Any())
                vertegenwoordigersLijst = await _magdaFacade.GetVertegenwoordigers(vertegenwoordigersArray, cancellationToken);
        }

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
            _clock.Today);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata);
        return CommandResult.Create(vCode, result);
    }

    //TODO move to MagdaFacade
    private static Vertegenwoordiger ToVertegenwoordiger(RegistreerVerenigingCommand.Vertegenwoordiger vert)
        => Vertegenwoordiger.Create(
            vert.Insz,
            vert.PrimairContactpersoon,
            vert.Roepnaam,
            vert.Rol,
            string.Empty,
            string.Empty);

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
