namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.VerwijderContactgegeven;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class VerwijderContactgegevenCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VerwijderContactgegevenCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderContactgegevenCommand> message,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(message.Command.VCode),
            message.Metadata
        );

        vereniging.VerwijderContactgegeven(message.Command.ContactgegevenId);

        var result = await _aggregateSession.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
