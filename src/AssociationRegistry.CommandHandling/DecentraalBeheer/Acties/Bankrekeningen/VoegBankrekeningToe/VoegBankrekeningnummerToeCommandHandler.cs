namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;

public class VoegBankrekeningnummerToeCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VoegBankrekeningnummerToeCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegBankrekeningnummerToeCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        var id = vereniging.VoegBankrekeningToe(envelope.Command.Bankrekeningnummer);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), id, result);
    }
}
