namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using Events;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using MartenDb.Store;
using Wolverine;

public class StartBewaartermijnMessageHandler
{
    public StartBewaartermijnMessageHandler()
    {
    }

    public async Task Handle(CommandEnvelope<StartBewaartermijnMessage> message, IEventStore eventStore, IMessageBus messageBus, BewaartermijnOptions bewaartermijnOptions, CancellationToken cancellationToken)
    {
        var bewaartermijnId = new BewaartermijnId(message.Command.VCode, message.Command.VertegenwoordigerId);
        var vervaldag = message.Metadata.Tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);

        // TODO: see if via message bus or scheduled task
        // await messageBus.ScheduleAsync(new VerwijderVertegenwoordigerPersoonsgegevensMessage(bewaartermijnId,
        //                                                                       bewaartermijnId.VCode,
        //                                                                       bewaartermijnId.VertegenwoordigerId,
        //                                                                       vervaldag), vervaldag.ToDateTimeOffset());

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
