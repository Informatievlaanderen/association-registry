namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;

using System;
using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Integrations.Slack;
using MartenDb.Store;
using Microsoft.Extensions.Logging;
using Notifications;
using Polly;

public class VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(
    IAggregateSession aggregateSession,
    INotifier notifier,
    ILogger<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler> logger
)
{
    public async Task Handle(
        VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand command,
        CancellationToken cancellationToken
    )
    {
        var retryCount = 4;

        await Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(x: 2, retryAttempt < retryCount ? retryAttempt : retryCount)),
                onRetryAsync: async (exception, _) =>
                {
                    logger.LogError(
                        exception,
                        $"{nameof(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler)} failed"
                    );
                    await notifier.Notify(
                        new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingGefaald(
                            exception,
                            command.VCode,
                            command.VCodeAuthentiekeVereniging
                        )
                    );
                }
            )
            .ExecuteAsync(() => VerwerkWeigeringDubbelDoorAuthentiekeVereniging(command, cancellationToken));
    }

    private async Task VerwerkWeigeringDubbelDoorAuthentiekeVereniging(
        VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand command,
        CancellationToken cancellationToken
    )
    {
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        var vereniging = await aggregateSession.Load<Vereniging>(
            command.VCode,
            metadata,
            allowDubbeleVereniging: true,
            allowVerwijderdeVereniging: true
        );

        vereniging.VerwerkWeigeringDubbelDoorAuthentiekeVereniging(command.VCodeAuthentiekeVereniging);

        await aggregateSession.Save(vereniging, metadata, cancellationToken);
    }
}
