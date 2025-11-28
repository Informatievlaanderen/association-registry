namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Reacties.VerwijderVertegenwoordigerPersoonsgegevens;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Events;
using Framework;
using Marten;
using MartenDb.Store;
using NodaTime;
using Persoonsgegevens;

public class VerwijderVertegenwoordigerPersoonsgegevensMessageHandler
{
    public async Task Handle(CommandEnvelope<VerwijderVertegenwoordigerPersoonsgegevensMessage> message, IDocumentSession session, IEventStore eventStore, IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository, CancellationToken cancellationToken)
    {
        var metadata = message.Metadata with
        {
            Tijdstip = SystemClock.Instance.GetCurrentInstant(),
            ExpectedVersion = null,
        }

        // we stoppen even het werk aan deze branch door verandering in prioriteiten
        // TODO:
        // Wolverine schedule message vanuit StartBewaartermijnMessageHandler (waarschijnlijk geen aparte queue mogelijk voor scheduled) @Koen uitzoeken
        // Controleren of scheduled message bij stoppen applicatie opgenomen worden (alternatief scheduled taks)
        // BewaartermijnWerdVerlopen & PersoonsgegevensWerdenVerwijderd in projecties
        // -> data naar 'NietMeerGekend' zetten
        // correlationId meegeven in event?
        // e2e (integratie) afwerken
        // Aggregate state bewaartermijn nodig?
        // Wat met concurrency?
        // Hebben we eventstore nodig?
        // Wat met domain invariant en preconditions

        vertegenwoordigerPersoonsgegevensRepository.Delete(VCode.Hydrate(message.Command.VCode), message.Command.VertegenwoordigerId);

        await eventStore.SaveTransactional(message.Command.BewaartermijnId,
                              null,
                              metadata,
                              cancellationToken,
                              [
                                  new BewaartermijnWerdVerlopen(
                                      message.Command.BewaartermijnId,
                                      message.Command.VCode,
                                      message.Command.VertegenwoordigerId,
                                      message.Command.Vervaldag),
                              ]);


        await eventStore.SaveTransactional(message.Command.VCode,
                              null,
                              metadata,
                              cancellationToken,
                              [
                                  new PersoonsgegevensWerdenVerwijderd(
                                      message.Command.BewaartermijnId,
                                      message.Command.VCode,
                                      message.Command.VertegenwoordigerId,
                                      message.Command.Vervaldag,
                                      message.Command.reden),
                              ]);

        await session.SaveChangesAsync(cancellationToken);
    }
}

public record PersoonsgegevensWerdenVerwijderd(string BewaartermijnId, string VCode, int VertegenwoordigerId, Instant Vervaldag, string Reden) : IEvent;

