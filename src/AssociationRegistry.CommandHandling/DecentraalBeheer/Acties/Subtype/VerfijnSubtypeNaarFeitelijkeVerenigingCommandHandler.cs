namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class VerfijnSubtypeNaarFeitelijkeVerenigingCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VerfijnSubtypeNaarFeitelijkeVerenigingCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerfijnSubtypeNaarFeitelijkeVerenigingCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        vereniging.VerfijnSubtypeNaarFeitelijkeVereniging();

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
