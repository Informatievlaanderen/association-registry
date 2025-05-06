namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Framework;
using Vereniging;

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
