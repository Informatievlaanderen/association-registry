namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegeven;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class WijzigContactgegevenCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public WijzigContactgegevenCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigContactgegevenCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        var (contacgegevenId, waarde, beschrijving, isPrimair) = envelope.Command.Contactgegeven;
        vereniging.WijzigContactgegeven(contacgegevenId, waarde, beschrijving, isPrimair);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
