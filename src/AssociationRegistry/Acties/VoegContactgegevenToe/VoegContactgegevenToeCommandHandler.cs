namespace AssociationRegistry.Acties.VoegContactgegevenToe;

using Framework;
using Vereniging;

public class VoegContactgegevenToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VoegContactgegevenToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VoegContactgegevenToeCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingRepository.Load<VerenigingOfAnyKind>(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        vereniging.VoegContactgegevenToe(envelope.Command.Contactgegeven);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
