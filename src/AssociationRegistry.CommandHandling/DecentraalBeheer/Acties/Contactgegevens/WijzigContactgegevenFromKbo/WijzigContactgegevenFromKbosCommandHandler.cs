namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegevenFromKbo;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class WijzigContactgegevenFromKboCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public WijzigContactgegevenFromKboCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigContactgegevenFromKboCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingMetRechtspersoonlijkheid>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        var (contactgegevenId, beschrijving, isPrimair) = envelope.Command.Contactgegeven;
        vereniging.WijzigContactgegeven(contactgegevenId, beschrijving, isPrimair);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
