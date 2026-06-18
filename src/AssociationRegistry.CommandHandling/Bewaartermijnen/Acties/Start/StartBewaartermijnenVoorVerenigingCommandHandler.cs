namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Start;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using Integrations.Slack;
using Microsoft.Extensions.Logging;
using Notifications;

public class StartBewaartermijnenVoorVerenigingCommandHandler
{
    public async Task Handle(
        StartBewaartermijnenVoorVerenigingCommand command,
        IEventStore eventStore,
        INotifier notifier,
        ILogger<StartBewaartermijnenVoorVerenigingCommandHandler> logger,
        CancellationToken cancellationToken
    )
    {
        var state = await eventStore.Load<VerenigingState>(command.StreamKey, expectedVersion: null);

        // TODO: Switch on PersoonsgegevensType if we have several e.g. bankrekeningnrs
        foreach (var vertegenwoordiger in state.Vertegenwoordigers)
        {
            try
            {
                var bewaartermijnId = new BewaartermijnId(
                    VCode.Create(command.StreamKey),
                    PersoonsgegevensType.Create(command.PersoonsgegevensType),
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
                            command.Vervaldag,
                            command.Reden
                        ),
                    ]
                );

                logger.LogInformation("Bewaartermijn started with id: {0}", bewaartermijnId);
            }
            catch (Marten.Exceptions.ExistingStreamIdCollisionException e)
            {
                await notifier.Notify(new EventSubscriptionBewaartermijnFailed(e.Message));
            }
            catch (Exception e)
            {
                await notifier.Notify(new EventSubscriptionBewaartermijnFailed(e.Message));

                throw;
            }
        }
    }
}
