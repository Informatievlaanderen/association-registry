namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Grar.Bewaartermijnen;
using AssociationRegistry.MartenDb.Store;

public class StartBewaartermijnMessageHandler
{
    public StartBewaartermijnMessageHandler()
    {
    }

    public async Task Handle(CommandEnvelope<StartBewaartermijnMessage> message, IEventStore eventStore, BewaartermijnOptions bewaartermijnOptions, CancellationToken cancellationToken)
    {
        var bewaartermijnId = new BewaartermijnId(message.Command.VCode, message.Command.VertegenwoordigerId);

        var vervaldag = message.Metadata.Tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await eventStore.SaveNew(bewaartermijnId,
                              message.Metadata,
                              cancellationToken,
                              [new BewaartermijnWerdGestart(
                                  bewaartermijnId,
                                  bewaartermijnId.VCode,
                                  bewaartermijnId.VertegenwoordigerId,
                                  vervaldag)]
        );
    }
}
