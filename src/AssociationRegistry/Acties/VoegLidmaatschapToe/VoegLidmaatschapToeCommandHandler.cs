namespace AssociationRegistry.Acties.VoegLidmaatschapToe;

using Framework;
using Vereniging;

public class VoegLidmaatschapToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VoegLidmaatschapToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<EntityCommandResult> Handle(CommandEnvelope<VoegLidmaatschapToeCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata.ExpectedVersion);

        var toegevoegdLidmaatschap = vereniging.VoegLidmaatschapToe(envelope.Command.Lidmaatschap);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), toegevoegdLidmaatschap.LidmaatschapId, result);
    }
}
