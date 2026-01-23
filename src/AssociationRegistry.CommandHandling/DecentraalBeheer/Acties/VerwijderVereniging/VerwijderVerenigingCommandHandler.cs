namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.VerwijderVereniging;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class VerwijderVerenigingCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VerwijderVerenigingCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderVerenigingCommand> message,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession
            .Load<Vereniging>(message.Command.VCode, message.Metadata)
            .OrWhenUnsupportedOperationForType()
            .Throw<VerenigingKanNietVerwijderdWorden>();

        vereniging.Verwijder(message.Command.Reden);

        var result = await _aggregateSession.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
