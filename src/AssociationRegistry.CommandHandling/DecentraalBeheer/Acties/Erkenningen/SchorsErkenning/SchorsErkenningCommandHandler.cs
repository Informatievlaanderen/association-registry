namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;

public class SchorsErkenningCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public SchorsErkenningCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<SchorsErkenningCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(envelope.Command.VCode, envelope.Metadata);

        vereniging.SchorsErkenning(envelope.Command.Erkenning, envelope.Metadata.Initiator);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
