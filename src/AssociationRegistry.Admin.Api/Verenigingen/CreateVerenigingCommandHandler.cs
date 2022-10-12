namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

public class CreateVerenigingCommandHandler: IRequestHandler<CommandEnvelope<CreateVerenigingCommand>>
{
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IVCodeService _vCodeService;

    public CreateVerenigingCommandHandler(IVerenigingsRepository verenigingsRepository, IVCodeService vCodeService)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
    }

    public async Task<Unit> Handle(CommandEnvelope<CreateVerenigingCommand> request, CancellationToken cancellationToken)
    {
        var vCode = await _vCodeService.GetNext();
        var vereniging = new Vereniging(vCode, request.Command.Naam);
        await _verenigingsRepository.Save(vereniging);
        return Unit.Value;
    }
}
