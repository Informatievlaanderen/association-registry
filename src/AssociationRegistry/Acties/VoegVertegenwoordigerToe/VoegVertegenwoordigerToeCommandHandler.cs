namespace AssociationRegistry.Acties.VoegVertegenwoordigerToe;

using Framework;
using Vereniging;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VoegVertegenwoordigerToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope)
    {
        var vereniging = await _verenigingRepository.Load(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        vereniging.VoegVertegenwoordigerToe(envelope.Command.Vertegenwoordiger);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
