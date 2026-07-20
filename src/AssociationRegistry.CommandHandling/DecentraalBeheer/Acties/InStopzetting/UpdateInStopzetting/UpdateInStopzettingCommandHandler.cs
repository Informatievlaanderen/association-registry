namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.InStopzetting.UpdateInStopzetting;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using Framework;
using MartenDb.Store;

public class UpdateInStopzettingCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public UpdateInStopzettingCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<UpdateInStopzettingCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        Throw<OvoCodeIsNietToegelatenDezeActieUitTeVoeren>.If(
            envelope.Metadata.Initiator != WellknownOvoNumbers.VloOvoCode,
            envelope.Metadata.Initiator
        );

        var vereniging = await _aggregateSession.Load<Vereniging>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata,
            allowDubbeleVereniging: true
        );

        if (envelope.Command.InStopzetting)
        {
            vereniging.ZetInStopzetting();
        }
        else
        {
            vereniging.ZetUitStopzetting();
        }

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
