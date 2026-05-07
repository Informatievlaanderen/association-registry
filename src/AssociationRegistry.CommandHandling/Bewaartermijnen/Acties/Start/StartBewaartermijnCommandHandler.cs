namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Events;
using Framework;
using Integrations.Slack;
using Notifications;

public class StartBewaartermijnCommandHandler
{
    public async Task Handle(
        StartBewaartermijnCommand command,
        IEventStore eventStore,
        INotifier notifier,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var bewaartermijnId = new BewaartermijnId(
                VCode.Create(command.StreamKey),
                PersoonsgegevensType.Create(command.PersoonsgegevensType),
                command.EntityId
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
                        command.Vervaldag,
                        command.Reden
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
