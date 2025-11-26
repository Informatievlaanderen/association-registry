namespace AssociationRegistry.CommandHandling.Bewaartermijnen;

using Acties.Start;
using Events;
using Framework;
using MartenDb.Store;

public class StartBewaartermijnMessageHandler
{
    public StartBewaartermijnMessageHandler()
    {

    }

    public async Task Handle(CommandEnvelope<StartBewaartermijnMessage> message, IEventStore eventStore, CancellationToken cancellationToken)
    {
        var bewaartermijnId = new BewaartermijnId(message.Command.VCode, message.Command.VertegenwoordigerId);

        await eventStore.Save(bewaartermijnId,
                              0,
                              message.Metadata,
                              cancellationToken,
                              new BewaartermijnWerdGestart(
                                  bewaartermijnId,
                                  bewaartermijnId.VCode,
                                  bewaartermijnId.VertegenwoordigerId)
        );
    }
}
