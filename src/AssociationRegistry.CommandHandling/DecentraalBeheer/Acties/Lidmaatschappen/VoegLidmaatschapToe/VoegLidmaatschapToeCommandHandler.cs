namespace AssociationRegistry.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;

using Framework;
using Vereniging;
using Vereniging.Exceptions;

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
            await _verenigingRepository.Load<VerenigingOfAnyKind>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata);

        if (await _verenigingRepository.IsVerwijderd(envelope.Command.Lidmaatschap.AndereVereniging))
            throw new VerenigingKanGeenLidWordenVanVerwijderdeVereniging();

        var toegevoegdLidmaatschap = vereniging.VoegLidmaatschapToe(envelope.Command.Lidmaatschap);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), toegevoegdLidmaatschap.LidmaatschapId, result);
    }
}
