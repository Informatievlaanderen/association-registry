namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.InStopzetting.UpdateInStopzetting;

using AssociationRegistry.DecentraalBeheer.Vereniging;
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
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata,
            allowDubbeleVereniging: true
        );

        if (envelope.Command.InStopzetting)
        {
            vereniging.ZetInStopzetting();
        }

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
