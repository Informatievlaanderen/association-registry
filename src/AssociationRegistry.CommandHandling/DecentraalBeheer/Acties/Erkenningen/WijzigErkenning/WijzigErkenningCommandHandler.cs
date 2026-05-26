namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using Framework;
using MartenDb.Store;

public class WijzigErkenningCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public WijzigErkenningCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigErkenningCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(envelope.Command.VCode, envelope.Metadata);

        var type = WijzigingsTypeErkenning.Create(envelope.Command.Erkenning.WijzigingsType);

        if (type.Value == WijzigingsTypeErkenning.CorrigeerValue)
            vereniging.CorrigeerErkenning(envelope.Command.Erkenning, envelope.Metadata.Initiator);

        if (type.Value == WijzigingsTypeErkenning.VerlengValue)
            vereniging.VerlengErkenning(envelope.Command.Erkenning, envelope.Metadata.Initiator);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
