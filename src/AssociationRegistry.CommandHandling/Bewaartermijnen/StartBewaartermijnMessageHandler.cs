namespace AssociationRegistry.CommandHandling.Bewaartermijnen;

using Acties.Start;
using Events;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using MartenDb.Store;
using NodaTime;

public class StartBewaartermijnMessageHandler
{
    public StartBewaartermijnMessageHandler()
    {
    }

    public async Task Handle(CommandEnvelope<StartBewaartermijnMessage> message, IEventStore eventStore, BewaartermijnOptions bewaartermijnOptions, CancellationToken cancellationToken)
    {
        var bewaartermijnId = new BewaartermijnId(message.Command.VCode, message.Command.VertegenwoordigerId);

        var vervaldag = message.Metadata.Tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await eventStore.Save(bewaartermijnId,
                              0,
                              message.Metadata,
                              cancellationToken,
                              new BewaartermijnWerdGestart(
                                  bewaartermijnId,
                                  bewaartermijnId.VCode,
                                  bewaartermijnId.VertegenwoordigerId,
                                  vervaldag)
        );
    }
}
