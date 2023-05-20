namespace AssociationRegistry.Acties.RegistreerVerenigingUitKbo;

using Framework;
using Vereniging;
using ResultNet;

public class RegistreerVerenigingUitKboCommandHandler
{
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingUitKboCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
    }

    public async Task<Result> Handle(CommandEnvelope<RegistreerVerenigingUitKboCommand> message, CancellationToken cancellationToken = default)
    {
        var command = message.Command;

        var vCode = await _vCodeService.GetNext();

        var vereniging = Vereniging.RegistreerVanuitKbo(
            vCode,
            command.KboNummer);

        var result = await _verenigingsRepository.Save(vereniging, message.Metadata, cancellationToken);
        return Result.Success(CommandResult.Create(vCode, result));
    }
}
