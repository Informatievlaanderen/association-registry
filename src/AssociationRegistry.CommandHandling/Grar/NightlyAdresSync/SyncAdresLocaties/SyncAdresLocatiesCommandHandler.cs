namespace AssociationRegistry.CommandHandling.Grar.NightlyAdresSync.SyncAdresLocaties;

using System;
using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using Integrations.Grar.AdresMatch;
using Integrations.Grar.Clients;
using MartenDb.Store;
using Microsoft.Extensions.Logging;

public class SyncAdresLocatiesCommandHandler(
    IAggregateSession aggregateSession,
    IGrarClient grarClient,
    ILogger<SyncAdresLocatiesCommandHandler> logger,
    IGeotagsService geotagsService
)
{
    public async Task Handle(SyncAdresLocatiesCommand locatiesCommand, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle {nameof(SyncAdresLocatiesCommandHandler)}");

        try
        {
            var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

            var vereniging = await aggregateSession.Load<VerenigingOfAnyKind>(
                VCode.Hydrate(locatiesCommand.VCode),
                metadata,
                allowDubbeleVereniging: true
            );

            await vereniging.SyncAdresLocaties(
                locatiesCommand.LocatiesWithAdres,
                locatiesCommand.IdempotenceKey,
                new GrarAddressVerrijkingsService(grarClient)
            );

            await vereniging.HerberekenGeotags(geotagsService);

            await aggregateSession.Save(vereniging, metadata, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Fout bij het synchroniseren van vereniging {VCode}", locatiesCommand.VCode);
        }
    }
}
