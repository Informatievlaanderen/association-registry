namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using System.Threading;
using System.Threading.Tasks;

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
