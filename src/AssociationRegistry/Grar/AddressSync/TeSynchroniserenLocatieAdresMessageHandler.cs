namespace AssociationRegistry.Grar.AddressSync;

using EventStore;
using Framework;
using Microsoft.Extensions.Logging;
using NodaTime;
using Vereniging;

public class TeSynchroniserenLocatieAdresMessageHandler(IVerenigingsRepository repository, IGrarClient grarClient, ILogger<TeSynchroniserenLocatieAdresMessageHandler> logger)
{
    public async Task Handle(TeSynchroniserenLocatieAdresMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle {nameof(TeSynchroniserenLocatieAdresMessageHandler)}");

        try
        {
            var vereniging = await repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode), allowDubbeleVereniging: true);

            await vereniging.SyncAdresLocaties(message.LocatiesWithAdres, message.IdempotenceKey, grarClient);

            await repository.Save(
                vereniging,
                new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                    SystemClock.Instance.GetCurrentInstant(),
                                    Guid.NewGuid()),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fout bij het synchroniseren van vereniging {VCode}", message.VCode);
        }
    }
}
