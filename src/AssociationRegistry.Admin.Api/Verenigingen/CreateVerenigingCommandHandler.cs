namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VCodes;
using VerenigingsNamen;

public class CreateVerenigingCommandHandler : IRequestHandler<CommandEnvelope<CreateVerenigingCommand>>
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
        var naam = new VerenigingsNaam(request.Command.Naam);
        var vereniging = new Vereniging(vCode, naam);
        await _verenigingsRepository.Save(vereniging);
        return Unit.Value;
    }
}
