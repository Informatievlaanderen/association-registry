namespace AssociationRegistry.Admin.AddressSync.Handlers;

using Infrastructure.Notifications;
using Fetchers;
using Grar.Models;
using Integrations.Slack;
using Marten;
using MessageHandling.Sqs.AddressSync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class SyncLocatieAdresHandler(
    ILogger<SyncLocatieAdresHandler> logger,
    ITeSynchroniserenLocatiesFetcher fetcher,
    ITeSynchroniserenLocatieAdresMessageHandler handler
)
    : ISyncLocatieAdresHandler
{
    public async Task<AdressSyncError[]> Handle(
        IDocumentSession session,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adressen synchroniseren werd gestart.");

        AdressSyncError[] errors = [];

        var adressenSyncMessages =
            await fetcher.GetTeSynchroniserenLocaties(session, cancellationToken);

        logger.LogInformation($"Er werden {adressenSyncMessages.Count()} synchroniseer berichten gevonden.");

        foreach (var message in adressenSyncMessages)
        {
            try
            {
                await handler.Handle(message, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError($"Adressen synchroniseren kon niet voltooid worden. {ex.Message}");

                errors = errors
                        .Append(new AdressSyncError(message.VCode,
                                                    message.LocatiesWithAdres.Select(x => x.LocatieId).ToList()))
                        .ToArray();
            }
        }

        LoggerExtensions.LogInformation(logger, $"Adressen synchroniseren werd voltooid.");

        return errors;
    }
}

public interface ISyncLocatieAdresHandler
{
    Task<AdressSyncError[]> Handle(IDocumentSession session, CancellationToken cancellationToken);
}
