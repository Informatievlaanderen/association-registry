namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
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
        IpdcProduct ipdcProduct,
        GegevensInitiator initiator,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(envelope.Command.VCode, envelope.Metadata);

        var id = vereniging.RegistreerErkenning(envelope.Command.Erkenning, ipdcProduct, initiator);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), id, result);
    }
}
