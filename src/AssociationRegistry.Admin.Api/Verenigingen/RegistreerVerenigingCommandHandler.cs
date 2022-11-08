namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading;
using System.Threading.Tasks;
using KboNummers;
using MediatR;
using VCodes;
using VerenigingsNamen;

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
        var vCode = await _vCodeService.GetNext();
        var vereniging = new Vereniging(vCode, naam, command.KorteNaam, command.KorteBeschrijving, command.Startdatum, kboNummer, _clock.Today);
        await _verenigingsRepository.Save(vereniging);
        return Unit.Value;
    }
}
