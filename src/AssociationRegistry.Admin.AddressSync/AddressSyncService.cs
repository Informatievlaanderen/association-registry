namespace AssociationRegistry.Admin.AddressSync;

using Grar.Models;
using Handlers;
using Infrastructure.Notifications;
using Integrations.Slack;
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public record NachtelijkeAdresSyncVolgensAdresId(string AdresId, List<LocatieIdWithVCode> LocatieIdWithVCodes);
public record NachtelijkeAdresSyncVolgensVCode(string VCode, List<LocatieWithAdres> LocatieWithAdres);
public record LocatieIdWithVCode(int LocatieId, string VCode);

public class AddressSyncService : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<AddressSyncService> _logger;
    private readonly INotifier _notifier;
    private readonly IDocumentSession _session;
    private readonly ISyncLocatieAdresProcessor _syncLocatieAdresProcessor;
    private readonly ISyncLocatieZonderAdresMatchProcessor _syncLocatieZonderAdresMatchProcessor;

    public AddressSyncService(
        ISyncLocatieAdresProcessor syncLocatieAdresProcessor,
        ISyncLocatieZonderAdresMatchProcessor syncLocatieZonderAdresMatchProcessor,
        IDocumentSession session,
        IHostApplicationLifetime hostApplicationLifetime,
        INotifier notifier,
        ILogger<AddressSyncService> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
        _notifier = notifier;
        _syncLocatieAdresProcessor = syncLocatieAdresProcessor;
        _syncLocatieZonderAdresMatchProcessor = syncLocatieZonderAdresMatchProcessor;
        _session = session;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
        => SyncAdressen(cancellationToken);

    public async Task SyncAdressen(CancellationToken cancellationToken)
    {
        AdressSyncError[] errors = [];

        try
        {
            var locatieAdresSyncErrors = await _syncLocatieAdresProcessor.Process(cancellationToken);
            errors = errors.Union(locatieAdresSyncErrors).ToArray();

            var locatieZonderAdresMatchSyncErrors = await _syncLocatieZonderAdresMatchProcessor.Process(
                cancellationToken
            );

            errors = errors.Union(locatieZonderAdresMatchSyncErrors).ToArray();

            if (errors.Any())
            {
                _logger.LogError($"Het synchroniseren van enkele locaties met het adressenregister is niet gelukt.");
                await _notifier.Notify(new AdresSynchronisatieProcessorGefaald(errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"AddressSyncService kon niet voltooid worden. {ex.Message}");
            await _notifier.Notify(new AdresSynchronisatieGefaald(ex));

            throw;
        }
        finally
        {
            await _session.DisposeAsync();
            _hostApplicationLifetime.StopApplication();
        }
    }
}

public record AdressSyncError(string VCode, List<int> LocatieIds, Exception Exception);
