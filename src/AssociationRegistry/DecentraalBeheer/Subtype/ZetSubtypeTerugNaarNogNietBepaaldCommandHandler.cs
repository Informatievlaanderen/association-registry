namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Framework;
using Vereniging;

public class ZetSubtypeTerugNaarNogNietBepaaldCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public ZetSubtypeTerugNaarNogNietBepaaldCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<ZetSubtypeTerugNaarNogNietBepaaldCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata.ExpectedVersion);

        vereniging.ZetSubtypeNaarNogNietBepaald();

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
