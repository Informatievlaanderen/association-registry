namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VerwijderLidmaatschap;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class VerwijderLidmaatschapCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VerwijderLidmaatschapCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderLidmaatschapCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        vereniging.VerwijderLidmaatschap(envelope.Command.LidmaatschapId);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
