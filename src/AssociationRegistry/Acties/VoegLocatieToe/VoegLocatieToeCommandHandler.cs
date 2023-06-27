namespace AssociationRegistry.Acties.VoegLocatieToe;

using Framework;
using Vereniging;

public class VoegLocatieToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VoegLocatieToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VoegLocatieToeCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingRepository.Load<Vereniging>(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        vereniging.VoegLocatieToe(envelope.Command.Locatie);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
