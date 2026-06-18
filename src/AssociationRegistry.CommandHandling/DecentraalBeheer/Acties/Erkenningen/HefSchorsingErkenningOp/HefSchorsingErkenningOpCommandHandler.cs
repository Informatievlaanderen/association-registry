namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.HefSchorsingErkenningOp;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;
using Wegwijs;

public class HefSchorsingErkenningOpCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public HefSchorsingErkenningOpCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<HefSchorsingErkenningOpCommand> envelope,
        IOrganisatieBevoegdheidService organisatieBevoegdheidService,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(envelope.Command.VCode, envelope.Metadata);

        await vereniging.HefSchorsingErkenningOp(envelope.Command.ErkenningId, envelope.Metadata.Initiator, organisatieBevoegdheidService);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
