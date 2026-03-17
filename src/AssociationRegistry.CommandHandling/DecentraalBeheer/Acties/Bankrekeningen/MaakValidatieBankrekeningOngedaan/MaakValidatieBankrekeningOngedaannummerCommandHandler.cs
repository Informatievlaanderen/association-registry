namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.MaakValidatieBankrekeningOngedaan;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.MartenDb.Store;

public class MaakValidatieBankrekeningnummerOngedaanCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public MaakValidatieBankrekeningnummerOngedaanCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<MaakValidatieBankrekeningnummerOngedaanCommand> envelope,
        CancellationToken cancellationToken = default)
    {
     if (envelope.Metadata.Initiator == CommandMetadata.VloOvoCode)
            throw new OvoCodeIsNietToegelatenDezeActieUitTeVoeren(envelope.Metadata.Initiator);

     var vereniging =
         await _aggregateSession.Load<VerenigingOfAnyKind>(
             VCode.Create(envelope.Command.VCode),
             envelope.Metadata);

     vereniging.MaakValidatieOngedaan(envelope.Command.BankrekeningnummerId, envelope.Metadata.Initiator);

     var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

     return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
