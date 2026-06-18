namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;
using Wegwijs;

public class WijzigErkenningCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public WijzigErkenningCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigErkenningCommand> envelope,
        IOrganisatieBevoegdheidService organisatieBevoegdheidService,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(envelope.Command.VCode, envelope.Metadata);

        await vereniging.WijzigErkenning(envelope.Command.Erkenning, envelope.Metadata.Initiator, organisatieBevoegdheidService);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
