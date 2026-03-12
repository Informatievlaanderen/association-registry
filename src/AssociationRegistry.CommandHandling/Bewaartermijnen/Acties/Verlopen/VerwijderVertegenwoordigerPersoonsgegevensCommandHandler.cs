namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Verlopen;

using Admin.Schema.Bewaartermijn;
using Admin.Schema.Persoonsgegevens;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using Start;
using AssociationRegistry.MartenDb.Store;
using Marten;

public class VerwijderVertegenwoordigerPersoonsgegevensCommandHandler
{

    public VerwijderVertegenwoordigerPersoonsgegevensCommandHandler()
    {
    }

   public async Task Handle(CommandEnvelope<VerwijderVertegenwoordigerPersoonsgegevensCommand> envelope, IDocumentSession session, BewaartermijnOptions bewaartermijnOptions, CancellationToken cancellationToken)
   {
        // var vertegenwoordigers = await session.Query<VertegenwoordigerPersoonsgegevensDocument>()
                                // .Where(x => x.VCode == envelope.Command.VCode && x.VertegenwoordigerId == envelope.Command.VertegenwoordigerId)
                                // .ToListAsync(cancellationToken);


        // vereniging.WijzigBankrekeningnummer(envelope.Command.Bankrekeningnummer, envelope.Metadata.Initiator);
        //
        // var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);
        //
        // return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
   }

    public async Task Handle(CommandEnvelope<VerwijderVertegenwoordigerPersoonsgegevensCommand> command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
