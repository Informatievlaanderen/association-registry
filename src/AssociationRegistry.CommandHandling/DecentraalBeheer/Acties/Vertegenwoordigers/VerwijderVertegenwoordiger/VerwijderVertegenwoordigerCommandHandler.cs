namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using MartenDb.Store;

public class VerwijderVertegenwoordigerCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VerwijderVertegenwoordigerCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
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

        var result = await _aggregateSession.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
