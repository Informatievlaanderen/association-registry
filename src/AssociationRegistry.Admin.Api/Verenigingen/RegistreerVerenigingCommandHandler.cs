namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VCodes;
using VerenigingsNamen;

public class RegistreerVerenigingCommandHandler : IRequestHandler<CommandEnvelope<RegistreerVerenigingCommand>>
{
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IVCodeService _vCodeService;

    public RegistreerVerenigingCommandHandler(IVerenigingsRepository verenigingsRepository, IVCodeService vCodeService)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
    }

    public async Task<Unit> Handle(CommandEnvelope<RegistreerVerenigingCommand> command, CancellationToken cancellationToken)
    {
        var naam = new VerenigingsNaam(command.Command.Naam);
        var vCode = await _vCodeService.GetNext();
        var vereniging = new Vereniging(vCode, naam);
        await _verenigingsRepository.Save(vereniging);
        return Unit.Value;
    }
}
