namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.WijzigLidmaatschap;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class WijzigLidmaatschapCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public WijzigLidmaatschapCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigLidmaatschapCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        vereniging.WijzigLidmaatschap(envelope.Command.Lidmaatschap);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
