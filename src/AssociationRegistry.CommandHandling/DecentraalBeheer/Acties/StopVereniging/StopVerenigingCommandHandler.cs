namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.StopVereniging;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class StopVerenigingCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IClock _clock;

    public StopVerenigingCommandHandler(IAggregateSession aggregateSession, IClock clock)
    {
        _aggregateSession = aggregateSession;
        _clock = clock;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<StopVerenigingCommand> message,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession
            .Load<Vereniging>(message.Command.VCode, message.Metadata)
            .OrWhenUnsupportedOperationForType()
            .Throw<VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden>();

        vereniging.Stop(message.Command.Einddatum, _clock);

        var result = await _aggregateSession.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
