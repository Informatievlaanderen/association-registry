namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;

public class VoegBankrekeningnummerToeCommandHandler
{
    private readonly IVerenigingsRepository _repository;

    public VoegBankrekeningnummerToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _repository = verenigingRepository;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegBankrekeningnummerToeCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _repository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata);

        var id = vereniging.VoegBankrekeningToe(envelope.Command.Bankrekeningnummer);

        var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), id, result);
    }
}
