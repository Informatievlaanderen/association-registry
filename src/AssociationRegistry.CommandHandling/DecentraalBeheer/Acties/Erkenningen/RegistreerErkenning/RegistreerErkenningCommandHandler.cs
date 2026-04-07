namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.MartenDb.Store;

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
        throw new NotImplementedException();
        // var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
        //     VCode.Create(envelope.Command.VCode),
        //     envelope.Metadata
        // );
        //
        // var id = vereniging.VoegBankrekeningToe(envelope.Command.Bankrekeningnummer);
        //
        // var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);
        //
        // return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), id, result);
    }
}
