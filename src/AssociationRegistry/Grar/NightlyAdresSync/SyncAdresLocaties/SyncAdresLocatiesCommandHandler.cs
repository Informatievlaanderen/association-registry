﻿namespace AssociationRegistry.Grar.NightlyAdresSync.SyncAdresLocaties;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Clients;
using Microsoft.Extensions.Logging;
using NodaTime;

public class SyncAdresLocatiesCommandHandler(
    IVerenigingsRepository repository,
    IGrarClient grarClient,
    ILogger<SyncAdresLocatiesCommandHandler> logger)
{
    public async Task Handle(SyncAdresLocatiesCommand locatiesCommand, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle {nameof(SyncAdresLocatiesCommandHandler)}");

        try
        {
            var vereniging = await repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(locatiesCommand.VCode), allowDubbeleVereniging: true);

            await vereniging.SyncAdresLocaties(locatiesCommand.LocatiesWithAdres, locatiesCommand.IdempotenceKey, grarClient);

            await repository.Save(
                vereniging,
                new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber,
                                    SystemClock.Instance.GetCurrentInstant(),
                                    Guid.NewGuid()),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Fout bij het synchroniseren van vereniging {VCode}", locatiesCommand.VCode);
        }
    }
}
