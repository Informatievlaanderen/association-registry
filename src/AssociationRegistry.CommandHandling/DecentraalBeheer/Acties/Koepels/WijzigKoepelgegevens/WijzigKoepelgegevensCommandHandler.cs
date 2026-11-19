namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Koepels.WijzigKoepelgegevens;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;

public class WijzigKoepelgegevensCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public WijzigKoepelgegevensCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigKoepelgegevensCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(envelope.Command.VCode, envelope.Metadata);

        vereniging.WijzigKoepelgegevens(envelope.Command.Koepelstatus, envelope.Command.PrivacyVerklaringGeaccepteerd);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(envelope.Command.VCode, result);
    }
}
