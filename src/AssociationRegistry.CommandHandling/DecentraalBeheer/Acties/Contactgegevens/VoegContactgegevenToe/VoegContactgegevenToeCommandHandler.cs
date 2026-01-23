namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.VoegContactgegevenToe;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class VoegContactgegevenToeCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VoegContactgegevenToeCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegContactgegevenToeCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        var contactgegeven = vereniging.VoegContactgegevenToe(envelope.Command.Contactgegeven);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(
            VCode.Create(envelope.Command.VCode),
            contactgegeven.ContactgegevenId,
            result
        );
    }
}
