namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Threading.Tasks;

public class CreateVerenigingCommandHandler
{
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IVNummerService _vNummerService;

    public CreateVerenigingCommandHandler(IVerenigingsRepository verenigingsRepository, IVNummerService vNummerService)
    {
        _verenigingsRepository = verenigingsRepository;
        _vNummerService = vNummerService;
    }

    public async Task Handle(CreateVerenigingCommand createVerenigingCommand)
    {
        var vNummer = await _vNummerService.GetNext();
        var vereniging = new Vereniging(vNummer, createVerenigingCommand.Naam);
        await _verenigingsRepository.Save(vereniging);
    }
}
