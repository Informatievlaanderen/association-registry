namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using Bewaartermijnen.Acties.Start;
using MartenDb.Store;
using Wolverine.Marten;

public class VerwijderVertegenwoordigerCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IMartenOutbox _outbox;

    public VerwijderVertegenwoordigerCommandHandler(IAggregateSession aggregateSession, IMartenOutbox outbox)
    {
        _aggregateSession = aggregateSession;
        _outbox = outbox;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderVertegenwoordigerCommand> message,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession
            .Load<Vereniging>(message.Command.VCode, message.Metadata)
            .OrWhenUnsupportedOperationForType()
            .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen>();

        vereniging.VerwijderVertegenwoordiger(message.Command.VertegenwoordigerId);

        await _outbox.SendAsync(
            new CommandEnvelope<StartBewaartermijnMessage>(
                new StartBewaartermijnMessage(message.Command.VCode, message.Command.VertegenwoordigerId),
                message.Metadata
            )
        );

        var result = await _aggregateSession.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
