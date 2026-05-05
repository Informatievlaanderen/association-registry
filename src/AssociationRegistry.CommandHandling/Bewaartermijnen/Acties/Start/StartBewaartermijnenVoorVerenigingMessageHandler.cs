namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using Integrations.Slack;
using Notifications;

public class StartBewaartermijnenVoorVerenigingMessageHandler
{
    public async Task Handle(
        StartBewaartermijnenVoorVerenigingMessage message,
        IEventStore eventStore,
        INotifier notifier,
        CancellationToken cancellationToken
    )
    {
        var state = await eventStore.Load<VerenigingState>(@message.StreamKey, expectedVersion: null);

        // TODO: Switch on PersoonsgegevensType if we have several e.g. bankrekeningnrs
        foreach (var vertegenwoordiger in state.Vertegenwoordigers)
        {
            try
            {
                var bewaartermijnId = new BewaartermijnId(
                    VCode.Create(message.StreamKey),
                    PersoonsgegevensType.Create(message.PersoonsgegevensType),
                    vertegenwoordiger.VertegenwoordigerId
                );

                await eventStore.SaveNew(
                    bewaartermijnId,
                    CommandMetadata.ForDigitaalVlaanderenProcess,
                    cancellationToken,
                    [
                        new BewaartermijnWerdGestartV2(
                            bewaartermijnId,
                            bewaartermijnId.VCode!,
                            bewaartermijnId.PersoonsgegevensType.Value,
                            bewaartermijnId.EntityId,
                            message.Vervaldag,
                            message.Reden
                        ),
                    ]
                );
            }
            catch (Exception e)
            {
                await notifier.Notify(new EventSubscriptionBewaartermijnFailed(e.Message));

                throw;
            }
        }
    }
}
