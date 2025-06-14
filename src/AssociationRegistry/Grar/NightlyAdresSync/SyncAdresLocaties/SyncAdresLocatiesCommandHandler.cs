﻿namespace AssociationRegistry.Grar.NightlyAdresSync.SyncAdresLocaties;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Clients;
using Microsoft.Extensions.Logging;
using Vereniging.Geotags;

public class SyncAdresLocatiesCommandHandler(
    IVerenigingsRepository repository,
    IGrarClient grarClient,
    ILogger<SyncAdresLocatiesCommandHandler> logger,
    IGeotagsService geotagsService)
{
    public async Task Handle(SyncAdresLocatiesCommand locatiesCommand, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle {nameof(SyncAdresLocatiesCommandHandler)}");

        try
        {
            var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

            var vereniging = await repository.Load<VerenigingOfAnyKind>(VCode.Hydrate(locatiesCommand.VCode), metadata, allowDubbeleVereniging: true);

            await vereniging.SyncAdresLocaties(locatiesCommand.LocatiesWithAdres, locatiesCommand.IdempotenceKey, grarClient);

            await vereniging.HerberekenGeotags(geotagsService);

            await repository.Save(
                vereniging,
                metadata,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Fout bij het synchroniseren van vereniging {VCode}", locatiesCommand.VCode);
        }
    }
}
