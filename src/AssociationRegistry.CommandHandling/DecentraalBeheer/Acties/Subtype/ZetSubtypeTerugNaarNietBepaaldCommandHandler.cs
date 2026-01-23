namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class ZetSubtypeTerugNaarNietBepaaldCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public ZetSubtypeTerugNaarNietBepaaldCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<ZetSubtypeTerugNaarNietBepaaldCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        vereniging.ZetSubtypeNaarNietBepaald();

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
