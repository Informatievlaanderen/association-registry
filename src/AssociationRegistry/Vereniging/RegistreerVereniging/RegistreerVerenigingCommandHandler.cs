namespace AssociationRegistry.Vereniging.RegistreerVereniging;

using ContactInfo;
using Framework;
using KboNummers;
using Locaties;
using Startdatums;
using VCodes;
using VerenigingsNamen;

public class RegistreerVerenigingCommandHandler
{
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IVCodeService _vCodeService;
    private readonly IClock _clock;

    public RegistreerVerenigingCommandHandler(IVerenigingsRepository verenigingsRepository, IVCodeService vCodeService, IClock clock)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
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
        var vCode = await _vCodeService.GetNext();
        var vereniging = Vereniging.Registreer(vCode, naam, command.KorteNaam, command.KorteBeschrijving, startdatum, kboNummer, contactInfoLijst, locatieLijst, _clock.Today);
        var result = await _verenigingsRepository.Save(vereniging, message.Metadata);
        return CommandResult.Create(vCode, result);
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
