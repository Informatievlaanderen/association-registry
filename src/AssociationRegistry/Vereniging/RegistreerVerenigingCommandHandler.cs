namespace AssociationRegistry.Vereniging;

using System.Threading;
using System.Threading.Tasks;
using ContactInfo;
using KboNummers;
using Startdatums;
using VCodes;
using VerenigingsNamen;
using Framework;
using MediatR;

public class RegistreerVerenigingCommandHandler : IRequestHandler<CommandEnvelope<RegistreerVerenigingCommand>>
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

    public async Task<Unit> Handle(CommandEnvelope<RegistreerVerenigingCommand> envelope, CancellationToken cancellationToken)
    {
        var command = envelope.Command;
        var naam = new VerenigingsNaam(command.Naam);
        var kboNummer = KboNummer.Create(command.KboNummber);
        var startdatum = Startdatum.Create(_clock, command.Startdatum);
        var contacten = ContactLijst.Create(command.Contacten);
        var vCode = await _vCodeService.GetNext();
        var vereniging = new Vereniging(vCode, naam, command.KorteNaam, command.KorteBeschrijving, startdatum, kboNummer, contacten, _clock.Today);
        await _verenigingsRepository.Save(vereniging, envelope.Metadata);
        return Unit.Value;
    }
}
