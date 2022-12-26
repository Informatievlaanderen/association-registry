namespace AssociationRegistry.Vereniging;

using System.Threading;
using System.Threading.Tasks;
using ContactInfo;
using KboNummers;
using Startdatums;
using VCodes;
using VerenigingsNamen;
using Framework;
using Locaties;
using MediatR;

public class RegistreerVerenigingCommandHandler : IRequestHandler<CommandEnvelope<RegistreerVerenigingCommand>,  RegistratieResult>
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

    public async Task<RegistratieResult> Handle(CommandEnvelope<RegistreerVerenigingCommand> envelope, CancellationToken cancellationToken)
    {
        var command = envelope.Command;
        var naam = new VerenigingsNaam(command.Naam);
        var kboNummer = KboNummer.Create(command.KboNummber);
        var startdatum = Startdatum.Create(_clock, command.Startdatum);
        var locatieLijst = LocatieLijst.CreateInstance(command.Locaties!.Select(ToLocatie));
        var contacten = ContactLijst.Create(command.ContactInfoLijst);
        var vCode = await _vCodeService.GetNext();
        var vereniging = new Vereniging(vCode, naam, command.KorteNaam, command.KorteBeschrijving, startdatum, kboNummer, contacten,locatieLijst, _clock.Today);
        var sequence = await _verenigingsRepository.Save(vereniging, envelope.Metadata);
        return new RegistratieResult(vCode, sequence);
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
            loc.HoofdLocatie,
            loc.LocatieType);
}
