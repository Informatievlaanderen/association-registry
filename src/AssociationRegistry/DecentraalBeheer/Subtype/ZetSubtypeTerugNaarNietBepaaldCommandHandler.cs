namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Framework;
using Vereniging;

public class ZetSubtypeTerugNaarNietBepaaldCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public ZetSubtypeTerugNaarNietBepaaldCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<ZetSubtypeTerugNaarNietBepaaldCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata);

        vereniging.ZetSubtypeNaarNietBepaald();

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
