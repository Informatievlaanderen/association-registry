namespace AssociationRegistry.DecentraalBeheer.Dubbelbeheer.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Notifications;
using AssociationRegistry.Notifications.Messages;
using AssociationRegistry.Vereniging;
using Microsoft.Extensions.Logging;
using NodaTime;
using Polly;

public class VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler(
    IVerenigingsRepository repository,
    INotifier notifier,
    ILogger<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler> logger)
{
    public async Task Handle(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand command, CancellationToken cancellationToken)
    {
        var retryCount = 4;

        await Policy
             .Handle<Exception>()
             .WaitAndRetryAsync(
                  retryCount,
                  sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(x: 2, retryAttempt < retryCount ? retryAttempt : retryCount)),
                  onRetryAsync: async (exception, _) =>
                  {
                      logger.LogError(exception, $"{nameof(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommandHandler)} failed");
                      await notifier.Notify(new VerwerkWeigeringDubbelDoorAuthentiekeVerenigingGefaald(exception, command.VCode, command.VCodeAuthentiekeVereniging));

                  })
             .ExecuteAsync(() => VerwerkWeigeringDubbelDoorAuthentiekeVereniging(command, cancellationToken));
    }

    private async Task VerwerkWeigeringDubbelDoorAuthentiekeVereniging(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand command, CancellationToken cancellationToken)
    {
        var vereniging = await repository.Load<Vereniging>(command.VCode, allowDubbeleVereniging:true, allowVerwijderdeVereniging:true);

        vereniging.VerwerkWeigeringDubbelDoorAuthentiekeVereniging(command.VCodeAuthentiekeVereniging);

        await repository.Save(
            vereniging,
            new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                SystemClock.Instance.GetCurrentInstant(),
                                Guid.NewGuid()),
            cancellationToken);
    }
}
