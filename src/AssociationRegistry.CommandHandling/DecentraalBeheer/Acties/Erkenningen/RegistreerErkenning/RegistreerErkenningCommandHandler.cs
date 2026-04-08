namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;

public class RegistreerErkenningCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public RegistreerErkenningCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<RegistreerErkenningCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vCode = VCode.Create(envelope.Command.VCode);

        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
           vCode,
            envelope.Metadata
        );

        var id = vereniging.RegistreerErkenning(envelope.Command.Erkenning, vCode, envelope.Metadata.Initiator);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), id, result);
    }
}
