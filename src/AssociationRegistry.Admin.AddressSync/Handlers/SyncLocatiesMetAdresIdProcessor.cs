namespace AssociationRegistry.Admin.AddressSync.Handlers;

using Fetchers;
using Marten;
using MessageHandling.Sqs.AddressSync;
using Microsoft.Extensions.Logging;

public class SyncLocatiesMetAdresIdProcessor : ISyncLocatieAdresProcessor
{
    private readonly ITeSynchroniserenLocatiesFetcher _fetcher;
    private readonly ITeSynchroniserenLocatieAdresMessageHandler _handler;
    private readonly IDocumentSession _session;
    private readonly ILogger<SyncLocatiesMetAdresIdProcessor> _logger;

    public SyncLocatiesMetAdresIdProcessor(
        ITeSynchroniserenLocatiesFetcher fetcher,
        ITeSynchroniserenLocatieAdresMessageHandler handler,
        IDocumentSession session,
        ILogger<SyncLocatiesMetAdresIdProcessor> logger
    )
    {
        _fetcher = fetcher;
        _handler = handler;
        _session = session;
        _logger = logger;
    }

    public async Task<AdressSyncError[]> Process(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adressen synchroniseren werd gestart.");

        AdressSyncError[] errors = [];

        var adressenSyncMessages = await _fetcher.GetTeSynchroniserenLocaties(_session, cancellationToken);

        _logger.LogInformation($"Er werden {adressenSyncMessages.Count()} synchroniseer berichten gevonden.");

        foreach (var message in adressenSyncMessages)
        {
            try
            {
                await _handler.Handle(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Adressen synchroniseren kon niet voltooid worden. {ex.Message}");

                errors = errors
                        .Append(
                             new AdressSyncError(
                                 message.VCode,
                                 message.LocatiesWithAdres.Select(x => x.LocatieId).ToList(),
                                 ex
                             )
                         )
                        .ToArray();
            }
        }

        _logger.LogInformation("Adressen synchroniseren werd voltooid.");

        return errors;
    }
}

public interface ISyncLocatieAdresProcessor
{
    Task<AdressSyncError[]> Process(CancellationToken cancellationToken);
}
