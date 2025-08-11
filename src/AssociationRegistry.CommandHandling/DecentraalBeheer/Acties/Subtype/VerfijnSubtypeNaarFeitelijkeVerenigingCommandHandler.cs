namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using System.Threading;
using System.Threading.Tasks;

public class VerfijnSubtypeNaarFeitelijkeVerenigingCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VerfijnSubtypeNaarFeitelijkeVerenigingCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VerfijnSubtypeNaarFeitelijkeVerenigingCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata);

        vereniging.VerfijnSubtypeNaarFeitelijkeVereniging();

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
